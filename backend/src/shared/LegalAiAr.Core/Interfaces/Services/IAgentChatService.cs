using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Tool-aware chat service for the agentic loop. Streams typed events that the handler
/// uses to detect tool calls, execute tools, and re-invoke the model.
/// Coexists with <see cref="IChatService"/> which remains for non-tool use cases.
/// </summary>
public interface IAgentChatService
{
    /// <summary>
    /// Invokes the model with tool definitions and streams typed events.
    /// The caller is responsible for the agentic loop: when <see cref="DoneEvent.FinishReason"/>
    /// is <see cref="AgentFinishReason.ToolCalls"/>, execute the requested tools, append
    /// tool result messages, and call this method again.
    /// </summary>
    /// <param name="messages">Full conversation history (system + user + assistant + tool messages).</param>
    /// <param name="tools">Tool definitions available to the model. Pass empty list to disable tools.</param>
    /// <param name="options">Model invocation options (temperature, max tokens).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async sequence of <see cref="AgentChatEvent"/>: text chunks, tool call requests, and a final done event.</returns>
    IAsyncEnumerable<AgentChatEvent> StreamWithToolsAsync(
        IReadOnlyList<AgentChatMessage> messages,
        IReadOnlyList<AgentToolDefinition> tools,
        AgentChatOptions options,
        CancellationToken cancellationToken = default);
}
