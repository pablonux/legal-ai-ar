using System.Runtime.CompilerServices;
using LegalAiAr.Application.Chat;
using LegalAiAr.Application.Chat.Commands.ChatQuery;
using LegalAiAr.Application.Chat.Pipeline;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LegalAiAr.Application.Tests.Chat.Pipeline;

public class ChatPipelineOrchestratorTests
{
    private static async Task<List<ChatStreamEvent>> CollectAsync(IAsyncEnumerable<ChatStreamEvent> source)
    {
        var list = new List<ChatStreamEvent>();
        await foreach (var item in source)
            list.Add(item);
        return list;
    }

    private static ChatQueryHandler CreateFakeExecutor(IAgentChatService agentChat, params string[] textChunks)
    {
        var toolRegistry = Substitute.For<IToolRegistry>();
        toolRegistry.GetToolDefinitions().Returns(Array.Empty<AgentToolDefinition>());
        agentChat.StreamWithToolsAsync(
                Arg.Any<IReadOnlyList<AgentChatMessage>>(),
                Arg.Any<IReadOnlyList<AgentToolDefinition>>(),
                Arg.Any<AgentChatOptions>(),
                Arg.Any<CancellationToken>())
            .Returns(TextStream(textChunks));
        return new ChatQueryHandler(
            agentChat,
            toolRegistry,
            Substitute.For<IServiceProvider>(),
            Options.Create(new ChatToolsOptions()),
            Substitute.For<ILogger<ChatQueryHandler>>());
    }

    private static ChatQueryHandler CreateFakeExecutor(params string[] textChunks)
    {
        return CreateFakeExecutor(Substitute.For<IAgentChatService>(), textChunks);
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

    private static ChatPipelineOrchestrator CreateSut(
        IEnumerable<IChatPipelineStage> stages,
        ChatQueryHandler executor)
    {
        return new ChatPipelineOrchestrator(
            stages,
            executor,
            Substitute.For<IServiceProvider>(),
            Options.Create(new ChatPipelineOptions()),
            Substitute.For<ILogger<ChatPipelineOrchestrator>>());
    }

    [global::Xunit.Fact]
    public async Task Handle_NoStages_PassesThrough()
    {
        var sut = CreateSut(Array.Empty<IChatPipelineStage>(), CreateFakeExecutor("Hello"));
        var events = await CollectAsync(sut.Handle(new ChatQueryCommand("q"), default));

        var chunks = events.OfType<ChatTextChunk>().Select(c => c.Text).ToList();
        Assert.Single(chunks);
        Assert.Equal("Hello", chunks[0]);
    }

    [global::Xunit.Fact]
    public async Task Handle_PreStreamShortCircuit_YieldsRejectionAndStops()
    {
        var agentChat = Substitute.For<IAgentChatService>();
        var executor = CreateFakeExecutor(agentChat, "should-not-run");

        var stage = Substitute.For<IChatPipelineStage>();
        stage.Name.Returns("RejectStage");
        stage.Phase.Returns(ChatPipelinePhase.PreStream);
        stage.IsEnabled(Arg.Any<ChatPipelineOptions>()).Returns(true);
        stage.ProcessAsync(Arg.Any<ChatPipelineContext>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(ChatPipelineResult.ShortCircuit(new ChatTextChunk("Rejected"))));

        var sut = CreateSut(new[] { stage }, executor);
        var events = await CollectAsync(sut.Handle(new ChatQueryCommand("q"), default));

        var chunks = events.OfType<ChatTextChunk>().Select(c => c.Text).ToList();
        Assert.Single(chunks);
        Assert.Equal("Rejected", chunks[0]);
        agentChat.DidNotReceive().StreamWithToolsAsync(
            Arg.Any<IReadOnlyList<AgentChatMessage>>(),
            Arg.Any<IReadOnlyList<AgentToolDefinition>>(),
            Arg.Any<AgentChatOptions>(),
            Arg.Any<CancellationToken>());
    }

    [global::Xunit.Fact]
    public async Task Handle_PreStreamContinues_ExecutorRuns()
    {
        var stage = Substitute.For<IChatPipelineStage>();
        stage.Name.Returns("PassStage");
        stage.Phase.Returns(ChatPipelinePhase.PreStream);
        stage.IsEnabled(Arg.Any<ChatPipelineOptions>()).Returns(true);
        stage.ProcessAsync(Arg.Any<ChatPipelineContext>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(ChatPipelineResult.Continue()));

        var sut = CreateSut(new[] { stage }, CreateFakeExecutor("Legal answer"));
        var events = await CollectAsync(sut.Handle(new ChatQueryCommand("q"), default));

        Assert.Contains(events, e => e is ChatTextChunk { Text: "Legal answer" });
    }

    [global::Xunit.Fact]
    public async Task Handle_PostStreamAppends_AddsEventsAfterExecutor()
    {
        var stage = Substitute.For<IChatPipelineStage>();
        stage.Name.Returns("PostStage");
        stage.Phase.Returns(ChatPipelinePhase.PostStream);
        stage.IsEnabled(Arg.Any<ChatPipelineOptions>()).Returns(true);
        stage.ProcessAsync(Arg.Any<ChatPipelineContext>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(ChatPipelineResult.Append(new ChatTextChunk("disclaimer"))));

        var sut = CreateSut(new[] { stage }, CreateFakeExecutor("Answer"));
        var events = await CollectAsync(sut.Handle(new ChatQueryCommand("q"), default));

        var texts = events.OfType<ChatTextChunk>().Select(c => c.Text).ToList();
        Assert.Equal(new[] { "Answer", "disclaimer" }, texts);
    }

    [global::Xunit.Fact]
    public async Task Handle_DisabledStage_Skipped()
    {
        var stage = Substitute.For<IChatPipelineStage>();
        stage.Name.Returns("OffStage");
        stage.Phase.Returns(ChatPipelinePhase.PreStream);
        stage.IsEnabled(Arg.Any<ChatPipelineOptions>()).Returns(false);

        var sut = CreateSut(new[] { stage }, CreateFakeExecutor("text"));
        var events = await CollectAsync(sut.Handle(new ChatQueryCommand("q"), default));

        await stage.DidNotReceive().ProcessAsync(Arg.Any<ChatPipelineContext>(), Arg.Any<CancellationToken>());
        var chunks = events.OfType<ChatTextChunk>().Select(c => c.Text).ToList();
        Assert.Single(chunks);
        Assert.Equal("text", chunks[0]);
    }
}
