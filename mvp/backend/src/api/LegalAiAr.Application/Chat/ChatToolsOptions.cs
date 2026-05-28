namespace LegalAiAr.Application.Chat;

/// <summary>
/// Configuration for the agentic chat loop. Bound from <c>ChatTools</c> config section.
/// </summary>
public class ChatToolsOptions
{
    public const string SectionName = "ChatTools";

    /// <summary>Max agentic loop iterations before forcing a final answer. Default 5.</summary>
    public int MaxIterations { get; set; } = 5;

    /// <summary>Per-tool execution timeout in seconds. Default 30.</summary>
    public int ToolTimeoutSeconds { get; set; } = 30;
}
