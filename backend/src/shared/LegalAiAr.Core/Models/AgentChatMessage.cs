namespace LegalAiAr.Core.Models;

/// <summary>
/// SDK-agnostic chat message for the agentic loop. The Infrastructure layer maps
/// these to/from <c>OpenAI.Chat.ChatMessage</c> instances.
/// </summary>
public sealed record AgentChatMessage
{
    public required AgentMessageRole Role { get; init; }

    /// <summary>
    /// Text content of the message. Null for assistant messages that only contain tool calls.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Tool calls requested by the assistant. Only set when <see cref="Role"/> is <see cref="AgentMessageRole.Assistant"/>.
    /// </summary>
    public IReadOnlyList<AgentToolCall>? ToolCalls { get; init; }

    /// <summary>
    /// ID of the tool call this message responds to. Only set when <see cref="Role"/> is <see cref="AgentMessageRole.Tool"/>.
    /// </summary>
    public string? ToolCallId { get; init; }

    public static AgentChatMessage System(string content) =>
        new() { Role = AgentMessageRole.System, Content = content };

    public static AgentChatMessage User(string content) =>
        new() { Role = AgentMessageRole.User, Content = content };

    public static AgentChatMessage Assistant(string? content, IReadOnlyList<AgentToolCall>? toolCalls = null) =>
        new() { Role = AgentMessageRole.Assistant, Content = content, ToolCalls = toolCalls };

    public static AgentChatMessage Tool(string toolCallId, string content) =>
        new() { Role = AgentMessageRole.Tool, Content = content, ToolCallId = toolCallId };
}

public enum AgentMessageRole
{
    System,
    User,
    Assistant,
    Tool
}

/// <summary>
/// A single tool call requested by the model within an assistant message.
/// </summary>
public sealed record AgentToolCall(
    string Id,
    string FunctionName,
    string ArgumentsJson);
