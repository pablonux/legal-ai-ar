namespace LegalAiAr.Application.Admin.Crawlers.DTOs;

/// <summary>
/// DTO for crawler configuration and status response.
/// </summary>
/// <param name="SourceId">FK to Sources (1=CSJN, 2=SAIJ, 3=PJN, 4=SCBA).</param>
/// <param name="SourceName">Short name from Sources.</param>
/// <param name="IsEnabled">Whether the source is enabled for crawling.</param>
/// <param name="LastCrawledAt">ISO 8601. Null if never executed.</param>
/// <param name="LastCrawledStatus">success, partial, failed. Null if never executed.</param>
/// <param name="LastDocumentCount">Documents processed in last crawl. Null if never executed.</param>
public record CrawlerConfigDto(
    int SourceId,
    string SourceName,
    bool IsEnabled,
    DateTime? LastCrawledAt,
    string? LastCrawledStatus,
    int? LastDocumentCount);
