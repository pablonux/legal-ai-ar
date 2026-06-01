using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Classifies a user query into guardrail categories. Implementations include
/// rule-based (fast, Application layer) and LLM-based (GPT-4o-mini, Infrastructure layer).
/// </summary>
public interface IGuardrailClassifier
{
    Task<GuardrailClassification> ClassifyAsync(
        string query, CancellationToken cancellationToken = default);
}
