namespace LegalAiAr.Core.Messages;

/// <summary>
/// Message for queue-crawler. Triggers a crawl run for a source.
/// </summary>
/// <param name="SourceId">Source to crawl (CSJN=1, SAIJ=2, PJN=3, SCBA=4).</param>
/// <param name="DocumentType">Kind of document: "ruling", "dictamen", "resolution", etc.</param>
/// <param name="Type">Type of ingestion: "incremental" or "by-range".</param>
/// <param name="Since">For incremental: crawl documents since this date. Null for by-range.</param>
/// <param name="DateFrom">For by-range: start of date range. Null for incremental.</param>
/// <param name="DateTo">For by-range: end of date range. Null for incremental.</param>
/// <param name="IngestionJobId">Pre-created job (e.g. from split). When set, worker uses existing job instead of creating new.</param>
/// <param name="UseCache">When true, check external download cache before HTTP calls. Always writes to cache (write-through).</param>
/// <param name="Reprocess">When true, skip deduplication checks (ExistsByExternalId, ExistsByContentHash) to allow reprocessing existing documents.</param>
/// <param name="MaxDocuments">Optional cap on documents to discover. Useful for partial runs (e.g. first 10 fallos-destacados).</param>
public record CrawlerMessage(
    int SourceId,
    string DocumentType = "ruling",
    string Type = "incremental",
    DateOnly? Since = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    Guid? IngestionJobId = null,
    bool UseCache = false,
    bool Reprocess = false,
    int? MaxDocuments = null);
