using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Crawlers.Commands.RunCrawler;

/// <summary>
/// Command to trigger a manual crawl for a source.
/// </summary>
/// <param name="SourceId">Source to crawl (CSJN=1, SAIJ=2, PJN=3, SCBA=4).</param>
/// <param name="DocumentType">Kind of document: "ruling", "dictamen", "resolution", etc.</param>
/// <param name="Type">Type of ingestion: "incremental" or "by-range".</param>
/// <param name="Since">For incremental: crawl documents since this date. Null for by-range or when using LastCrawledAt.</param>
/// <param name="DateFrom">For by-range: start of date range. Null for incremental.</param>
/// <param name="DateTo">For by-range: end of date range. Null for incremental.</param>
/// <param name="UseCache">When true, use cached external downloads instead of re-downloading from source.</param>
/// <param name="Reprocess">When true, skip deduplication checks to allow reprocessing existing documents.</param>
public record RunCrawlerCommand(
    int SourceId,
    string DocumentType,
    string Type,
    DateOnly? Since = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    bool UseCache = false,
    bool Reprocess = false,
    int? MaxDocuments = null,
    int? SkipDocuments = null) : IRequest<RunCrawlerResult>;
