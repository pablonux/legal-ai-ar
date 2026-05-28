namespace LegalAiAr.Core.Models;

/// <summary>
/// Validation result emitted by the output guardrail after response completion.
/// Streamed as <c>event: validation\ndata: {...}\n\n</c>.
/// </summary>
public sealed record ChatValidationEvent(
    string Status,
    int CitationsChecked,
    int Valid,
    int Warnings,
    IReadOnlyList<string> Details) : ChatStreamEvent;
