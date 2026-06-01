using System.Text;
using LegalAiAr.Core.Interfaces.Pipeline;

namespace LegalAiAr.Core.Models;

/// <summary>
/// Shared state passed through all chat pipeline stages. Carries the original query,
/// classification results, enrichment data, accumulated response, and tool execution context.
/// </summary>
public sealed class ChatPipelineContext
{
    public required string OriginalQuery { get; init; }
    public required ChatPipelineOptions Options { get; init; }
    public string? ClassifiedIntent { get; set; }
    public GuardrailClassification? InputClassification { get; set; }
    public QueryEnrichment? Enrichment { get; set; }
    public List<AgentChatMessage> Messages { get; init; } = [];
    public required ToolExecutionContext ToolContext { get; init; }
    public StringBuilder AccumulatedResponse { get; } = new();
    public bool IsShortCircuited { get; set; }

    /// <summary>
    /// Indicates whether the response contains substantive legal content
    /// (used by the finalizer to decide disclaimer injection).
    /// Set by the output guardrail stage.
    /// </summary>
    public bool NeedsDisclaimer { get; set; }
}
