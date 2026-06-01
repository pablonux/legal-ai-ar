namespace LegalAiAr.Application.Admin.Pipeline.Commands.BulkRequeue;

/// <param name="Queued">Number of rulings successfully queued.</param>
/// <param name="Skipped">Number of rulings skipped (missing FullText/BlobPath).</param>
/// <param name="Failed">Number of rulings that failed to publish.</param>
/// <param name="Stage">Target stage.</param>
public record BulkRequeueResult(
    int Queued,
    int Skipped,
    int Failed,
    string Stage);
