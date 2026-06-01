using System.Runtime.CompilerServices;
using LegalAiAr.Application.Chat;
using LegalAiAr.Application.Chat.Commands.ChatQuery;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LegalAiAr.Application.Tests.Chat.Commands.ChatQuery;

public class ChatQueryHandlerTests
{
    private readonly IAgentChatService _agentChat = Substitute.For<IAgentChatService>();
    private readonly IToolRegistry _toolRegistry = Substitute.For<IToolRegistry>();
    private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();

    private ChatQueryHandler CreateSut(ChatToolsOptions? options = null)
    {
        return new ChatQueryHandler(
            _agentChat,
            _toolRegistry,
            _serviceProvider,
            Options.Create(options ?? new ChatToolsOptions()),
            Substitute.For<ILogger<ChatQueryHandler>>());
    }

    private static async Task<List<ChatStreamEvent>> CollectAsync(
        IAsyncEnumerable<ChatStreamEvent> source, CancellationToken ct = default)
    {
        var list = new List<ChatStreamEvent>();
        await foreach (var item in source.WithCancellation(ct))
            list.Add(item);
        return list;
    }

    private static async IAsyncEnumerable<AgentChatEvent> TextStream(
        string[] chunks, [EnumeratorCancellation] CancellationToken ct = default)
    {
        await Task.Yield();
        foreach (var chunk in chunks)
        {
            ct.ThrowIfCancellationRequested();
            yield return new TextChunkEvent(chunk);
        }
        yield return new DoneEvent(AgentFinishReason.Stop, null);
    }

    private static async IAsyncEnumerable<AgentChatEvent> ToolCallStream(
        string toolCallId, string toolName, string argsJson,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await Task.Yield();
        yield return new ToolCallRequestEvent(toolCallId, toolName, argsJson);
        yield return new DoneEvent(AgentFinishReason.ToolCalls, null);
    }

    [Fact]
    public async Task Handle_ModelReturnsText_YieldsTextChunks()
    {
        _toolRegistry.GetToolDefinitions().Returns(Array.Empty<AgentToolDefinition>());
        _agentChat.StreamWithToolsAsync(
                Arg.Any<IReadOnlyList<AgentChatMessage>>(),
                Arg.Any<IReadOnlyList<AgentToolDefinition>>(),
                Arg.Any<AgentChatOptions>(),
                Arg.Any<CancellationToken>())
            .Returns(TextStream(["Hola", " mundo"]));

        var sut = CreateSut();
        var events = await CollectAsync(sut.Handle(new ChatQueryCommand("hola"), default));

        Assert.Equal(2, events.Count);
        Assert.IsType<ChatTextChunk>(events[0]);
        Assert.Equal("Hola", ((ChatTextChunk)events[0]).Text);
        Assert.Equal(" mundo", ((ChatTextChunk)events[1]).Text);
    }

    [Fact]
    public async Task Handle_ModelCallsTool_ExecutesToolAndReInvokes()
    {
        _toolRegistry.GetToolDefinitions().Returns(Array.Empty<AgentToolDefinition>());
        _toolRegistry.ExecuteAsync(
                "search_rulings", Arg.Any<string>(),
                Arg.Any<ToolExecutionContext>(), Arg.Any<CancellationToken>())
            .Returns("[search_rulings: 1 result]\n1. Case: Test | ID: abc");

        var callCount = 0;
        _agentChat.StreamWithToolsAsync(
                Arg.Any<IReadOnlyList<AgentChatMessage>>(),
                Arg.Any<IReadOnlyList<AgentToolDefinition>>(),
                Arg.Any<AgentChatOptions>(),
                Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                callCount++;
                if (callCount == 1)
                    return ToolCallStream("tc-1", "search_rulings", "{\"query\":\"test\"}");
                return TextStream(["Respuesta basada en ", "la jurisprudencia."]);
            });

        var sut = CreateSut();
        var events = await CollectAsync(sut.Handle(new ChatQueryCommand("test"), default));

        Assert.Contains(events, e => e is ChatToolStart { ToolName: "search_rulings" });
        Assert.Contains(events, e => e is ChatToolEnd { ToolName: "search_rulings" });
        Assert.Contains(events, e => e is ChatTextChunk { Text: "Respuesta basada en " });
        Assert.Equal(2, callCount);
    }

    [Fact]
    public async Task Handle_MaxIterationsReached_ForcesAnswer()
    {
        _toolRegistry.GetToolDefinitions().Returns(Array.Empty<AgentToolDefinition>());
        _toolRegistry.ExecuteAsync(
                Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<ToolExecutionContext>(), Arg.Any<CancellationToken>())
            .Returns("result");

        _agentChat.StreamWithToolsAsync(
                Arg.Any<IReadOnlyList<AgentChatMessage>>(),
                Arg.Any<IReadOnlyList<AgentToolDefinition>>(),
                Arg.Any<AgentChatOptions>(),
                Arg.Any<CancellationToken>())
            .Returns(
                ToolCallStream("tc-1", "tool", "{}"),
                ToolCallStream("tc-2", "tool", "{}"),
                TextStream(["Forced answer"]));

        var sut = CreateSut(new ChatToolsOptions { MaxIterations = 2 });
        var events = await CollectAsync(sut.Handle(new ChatQueryCommand("test"), default));

        Assert.Contains(events, e => e is ChatTextChunk { Text: "Forced answer" });
    }

    [Fact]
    public async Task Handle_QueryExceedsMaxLength_Truncates()
    {
        var longQuery = new string('x', 1500);
        _toolRegistry.GetToolDefinitions().Returns(Array.Empty<AgentToolDefinition>());
        _agentChat.StreamWithToolsAsync(
                Arg.Any<IReadOnlyList<AgentChatMessage>>(),
                Arg.Any<IReadOnlyList<AgentToolDefinition>>(),
                Arg.Any<AgentChatOptions>(),
                Arg.Any<CancellationToken>())
            .Returns(TextStream(["ok"]));

        var sut = CreateSut();
        await CollectAsync(sut.Handle(new ChatQueryCommand(longQuery), default));

        _agentChat.Received(1).StreamWithToolsAsync(
            Arg.Is<IReadOnlyList<AgentChatMessage>>(msgs =>
                msgs.Any(m => m.Role == AgentMessageRole.User && m.Content!.Length == 1000)),
            Arg.Any<IReadOnlyList<AgentToolDefinition>>(),
            Arg.Any<AgentChatOptions>(),
            Arg.Any<CancellationToken>());
    }
}
