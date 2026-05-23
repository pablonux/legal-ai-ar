namespace LegalAiAr.Application.Admin.Jobs.DTOs;

/// <summary>
/// Pipeline status for a single source.
/// </summary>
/// <param name="SourceId">Source ID (1=CSJN, 2=SAIJ, etc.).</param>
/// <param name="SourceName">Short name from Sources.</param>
/// <param name="LastCrawledAt">When last crawl completed. Null if never.</param>
/// <param name="LastCrawledStatus">success, partial, failed. Null if never.</param>
/// <param name="LastDocumentCount">Documents in last crawl. Null if never.</param>
/// <param name="QueueLength">Approximate message count in crawler queue.</param>
public record PipelineSourceStatusDto(
    int SourceId,
    string SourceName,
    DateTime? LastCrawledAt,
    string? LastCrawledStatus,
    int? LastDocumentCount,
    int QueueLength);
