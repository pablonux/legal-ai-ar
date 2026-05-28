using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// A single stage in the chat processing pipeline. Stages wrap the agentic executor
/// with pre-stream (guardrails, enrichment), chunk-mode (normalization), and
/// post-stream (validation, disclaimer) processing.
/// </summary>
public interface IChatPipelineStage
{
    string Name { get; }
    ChatPipelinePhase Phase { get; }

    bool IsEnabled(ChatPipelineOptions options);

    Task<ChatPipelineResult> ProcessAsync(
        ChatPipelineContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Configuration options for the chat pipeline. Each stage checks its sub-section
/// via <see cref="IChatPipelineStage.IsEnabled"/>.
/// </summary>
public class ChatPipelineOptions
{
    public const string SectionName = "ChatPipeline";

    public InputGuardrailOptions InputGuardrail { get; set; } = new();
    public QueryEnricherOptions QueryEnricher { get; set; } = new();
    public OutputGuardrailOptions OutputGuardrail { get; set; } = new();
    public ResponseFinalizerOptions ResponseFinalizer { get; set; } = new();
}

public class InputGuardrailOptions
{
    public bool Enabled { get; set; } = true;
    public bool UseLlmClassifier { get; set; } = true;
}

public class QueryEnricherOptions
{
    public bool Enabled { get; set; } = true;
    public bool UseLlmFallback { get; set; } = true;
}

public class OutputGuardrailOptions
{
    public bool Enabled { get; set; } = true;
    public string Strictness { get; set; } = "moderate";
}

public class ResponseFinalizerOptions
{
    public bool Enabled { get; set; } = true;
    public bool DisclaimerEnabled { get; set; } = true;
    public string DisclaimerText { get; set; } = "Esta información es de carácter referencial y no constituye asesoramiento legal. Consultá con un profesional del derecho para tu caso particular.";
    public bool StructureEnforcement { get; set; }
}
