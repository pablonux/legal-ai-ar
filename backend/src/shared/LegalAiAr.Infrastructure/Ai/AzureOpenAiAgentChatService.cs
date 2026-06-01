using System.ClientModel;
using System.Runtime.CompilerServices;
using System.Text;
using Azure.AI.OpenAI;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace LegalAiAr.Infrastructure.Ai;

/// <summary>
/// Tool-aware chat service wrapping ChatDeploymentName (GPT-5) with function calling support.
/// Handles streaming tool call accumulation and emits typed <see cref="AgentChatEvent"/> instances.
/// </summary>
public sealed class AzureOpenAiAgentChatService : IAgentChatService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<AzureOpenAiAgentChatService> _logger;

    public AzureOpenAiAgentChatService(
        IOptions<AzureOpenAiOptions> options,
        ILogger<AzureOpenAiAgentChatService> logger)
    {
        _logger = logger;
        var opts = options.Value;

        if (string.IsNullOrWhiteSpace(opts.Endpoint))
            throw new InvalidOperationException("AzureOpenAI:Endpoint is required.");
        if (string.IsNullOrWhiteSpace(opts.ApiKey))
            throw new InvalidOperationException("AzureOpenAI:ApiKey is required.");
        if (string.IsNullOrWhiteSpace(opts.ChatDeploymentName))
            throw new InvalidOperationException("AzureOpenAI:ChatDeploymentName is required.");

        var credential = new ApiKeyCredential(opts.ApiKey);
        var azureClient = new AzureOpenAIClient(new Uri(opts.Endpoint.TrimEnd('/') + "/"), credential);
        _chatClient = azureClient.GetChatClient(opts.ChatDeploymentName);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<AgentChatEvent> StreamWithToolsAsync(
        IReadOnlyList<AgentChatMessage> messages,
        IReadOnlyList<AgentToolDefinition> tools,
        AgentChatOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var chatMessages = messages.Select(MapToChatMessage).ToList();
        var chatOptions = BuildChatOptions(options, tools);

        var pendingToolCalls = new Dictionary<int, ToolCallAccumulator>();
        ChatFinishReason? finishReason = null;
        ChatTokenUsage? tokenUsage = null;

        _logger.LogDebug("Invoking chat model with {ToolCount} tool definitions", tools.Count);

        await foreach (var update in _chatClient.CompleteChatStreamingAsync(chatMessages, chatOptions, cancellationToken))
        {
            foreach (var content in update.ContentUpdate)
            {
                var text = content?.Text;
                if (!string.IsNullOrEmpty(text))
                    yield return new TextChunkEvent(text);
            }

            foreach (var toolUpdate in update.ToolCallUpdates)
            {
                if (!pendingToolCalls.TryGetValue(toolUpdate.Index, out var acc))
                {
                    acc = new ToolCallAccumulator();
                    pendingToolCalls[toolUpdate.Index] = acc;
                }

                if (!string.IsNullOrEmpty(toolUpdate.ToolCallId))
                    acc.Id = toolUpdate.ToolCallId;
                if (!string.IsNullOrEmpty(toolUpdate.FunctionName))
                    acc.Name = toolUpdate.FunctionName;
                if (toolUpdate.FunctionArgumentsUpdate is { } argsFragment)
                    acc.Args.Append(argsFragment.ToString());
            }

            if (update.FinishReason is not null)
                finishReason = update.FinishReason;
            if (update.Usage is not null)
                tokenUsage = update.Usage;
        }

        foreach (var (_, acc) in pendingToolCalls.OrderBy(kv => kv.Key))
        {
            _logger.LogDebug("Tool call accumulated: {ToolName}({ArgsLength} chars)",
                acc.Name, acc.Args.Length);
            yield return new ToolCallRequestEvent(acc.Id, acc.Name, acc.Args.ToString());
        }

        yield return new DoneEvent(MapFinishReason(finishReason), MapTokenUsage(tokenUsage));
    }

    private static ChatMessage MapToChatMessage(AgentChatMessage msg) => msg.Role switch
    {
        AgentMessageRole.System =>
            ChatMessage.CreateSystemMessage(msg.Content ?? string.Empty),

        AgentMessageRole.User =>
            ChatMessage.CreateUserMessage(msg.Content ?? string.Empty),

        AgentMessageRole.Assistant when msg.ToolCalls?.Count > 0 =>
            ChatMessage.CreateAssistantMessage(
                msg.ToolCalls.Select(tc =>
                    ChatToolCall.CreateFunctionToolCall(
                        tc.Id,
                        tc.FunctionName,
                        BinaryData.FromString(tc.ArgumentsJson)))),

        AgentMessageRole.Assistant =>
            ChatMessage.CreateAssistantMessage(msg.Content ?? string.Empty),

        AgentMessageRole.Tool =>
            ChatMessage.CreateToolMessage(msg.ToolCallId!, msg.Content ?? string.Empty),

        _ => throw new ArgumentOutOfRangeException(nameof(msg), msg.Role, "Unknown agent message role.")
    };

    private static ChatCompletionOptions BuildChatOptions(
        AgentChatOptions options,
        IReadOnlyList<AgentToolDefinition> tools)
    {
        var chatOptions = new ChatCompletionOptions
        {
            Temperature = options.Temperature,
            MaxOutputTokenCount = options.MaxOutputTokens
        };

        foreach (var tool in tools)
        {
            chatOptions.Tools.Add(ChatTool.CreateFunctionTool(
                tool.Name,
                tool.Description,
                BinaryData.FromString(tool.ParametersSchema)));
        }

        return chatOptions;
    }

    private static AgentFinishReason MapFinishReason(ChatFinishReason? reason)
    {
        if (reason == ChatFinishReason.Stop) return AgentFinishReason.Stop;
        if (reason == ChatFinishReason.ToolCalls) return AgentFinishReason.ToolCalls;
        if (reason == ChatFinishReason.Length) return AgentFinishReason.Length;
        if (reason == ChatFinishReason.ContentFilter) return AgentFinishReason.ContentFilter;
        return AgentFinishReason.Stop;
    }

    private static AgentTokenUsage? MapTokenUsage(ChatTokenUsage? usage)
    {
        if (usage is null) return null;
        return new AgentTokenUsage(
            usage.InputTokenCount,
            usage.OutputTokenCount,
            usage.TotalTokenCount);
    }

    private sealed class ToolCallAccumulator
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public StringBuilder Args { get; } = new();
    }
}
