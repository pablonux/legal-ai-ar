namespace LegalAiAr.Core.Models;

/// <summary>
/// Aggregated counts for a single pipeline stage within a job.
/// </summary>
public record StageSummary(
    int Pending,
    int Processing,
    int Completed,
    int Failed,
    int Discarded,
    int Cancelled);
