namespace LegalAiAr.Core.Models;

/// <summary>
/// Events yielded by the agentic <c>ChatQueryHandler</c> to the controller for SSE serialization.
/// </summary>
public abstract record ChatStreamEvent;

/// <summary>
/// Text fragment from the model response. Streamed as <c>data: {text}\n\n</c>.
/// </summary>
public sealed record ChatTextChunk(string Text) : ChatStreamEvent;

/// <summary>
/// A tool execution has started. Streamed as <c>event: tool_start\ndata: {"tool":"name"}\n\n</c>.
/// </summary>
public sealed record ChatToolStart(string ToolName) : ChatStreamEvent;

/// <summary>
/// A tool execution has completed. Streamed as <c>event: tool_end\ndata: {"tool":"name","resultCount":N}\n\n</c>.
/// </summary>
public sealed record ChatToolEnd(string ToolName, int ResultCount) : ChatStreamEvent;

/// <summary>
/// Post-stream correction: carries the normalized full response text after citation and markdown cleanup.
/// Streamed as <c>event: normalized\ndata: {text}\n\n</c>. Clients can optionally replace accumulated display.
/// </summary>
public sealed record ChatNormalizedResponse(string Text) : ChatStreamEvent;
