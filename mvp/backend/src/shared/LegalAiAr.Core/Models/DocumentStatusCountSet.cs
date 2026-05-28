namespace LegalAiAr.Core.Models;

/// <summary>
/// Aggregated <see cref="Enums.DocumentStatus"/> counts for documents in one ingestion job.
/// </summary>
public sealed class DocumentStatusCountSet
{
    public int Pending { get; init; }
    public int Processing { get; init; }
    public int Completed { get; init; }
    public int Failed { get; init; }
    public int Discarded { get; init; }
    public int Cancelled { get; init; }

    public int Total => Pending + Processing + Completed + Failed + Discarded + Cancelled;

    public int OutstandingPendingOrProcessing => Pending + Processing;
}
