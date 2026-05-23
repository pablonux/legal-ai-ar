namespace LegalAiAr.Core.Models;

/// <summary>
/// Result of the input guardrail classification of a user query.
/// </summary>
public sealed record GuardrailClassification(
    GuardrailCategory Category,
    GuardrailSource Source,
    float? Confidence);

/// <summary>
/// Classification categories for the input guardrail.
/// </summary>
public enum GuardrailCategory
{
    LegalQuery,
    Greeting,
    Clarification,
    OutOfScope,
    Harmful
}

/// <summary>
/// Source that produced the guardrail classification.
/// </summary>
public enum GuardrailSource
{
    RuleBased,
    LlmClassifier
}
