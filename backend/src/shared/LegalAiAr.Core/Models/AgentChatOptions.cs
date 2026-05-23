namespace LegalAiAr.Core.Models;

/// <summary>
/// Configuration for a single agentic chat model invocation.
/// </summary>
public sealed record AgentChatOptions
{
    public float Temperature { get; init; } = 0.3f;
    public int MaxOutputTokens { get; init; } = 2048;
}
