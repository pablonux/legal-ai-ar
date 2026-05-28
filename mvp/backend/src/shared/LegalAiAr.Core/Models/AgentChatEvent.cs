namespace LegalAiAr.Core.Models;

/// <summary>
/// Base type for events emitted by the agentic chat service during tool-aware streaming.
/// </summary>
public abstract record AgentChatEvent;

/// <summary>
/// Streamed text fragment from the model response.
/// </summary>
public sealed record TextChunkEvent(string Text) : AgentChatEvent;

/// <summary>
/// The model requests execution of a tool. The handler must resolve the tool,
/// execute it, and append the result as a Tool message before re-invoking the model.
/// </summary>
public sealed record ToolCallRequestEvent(
    string ToolCallId,
    string ToolName,
    string ArgumentsJson) : AgentChatEvent;

/// <summary>
/// The model finished generating. Check <see cref="FinishReason"/> to determine
/// whether to continue the agentic loop (ToolCalls) or stream the final response (Stop).
/// </summary>
public sealed record DoneEvent(
    AgentFinishReason FinishReason,
    AgentTokenUsage? Usage) : AgentChatEvent;

/// <summary>
/// Reason the model stopped generating. Maps to OpenAI finish_reason without SDK dependency.
/// </summary>
public enum AgentFinishReason
{
    Stop,
    ToolCalls,
    Length,
    ContentFilter
}

/// <summary>
/// Token consumption for a single model invocation.
/// </summary>
public sealed record AgentTokenUsage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens);
