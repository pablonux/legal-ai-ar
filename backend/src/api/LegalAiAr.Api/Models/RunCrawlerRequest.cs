namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for POST /api/admin/crawlers/{sourceId}/run.
/// </summary>
/// <param name="DocumentType">Kind of document: "ruling", "dictamen", "resolution". Defaults to "ruling".</param>
/// <param name="Type">Type of crawl: "incremental" or "by-range".</param>
/// <param name="Since">For incremental: crawl documents since this date (YYYY-MM-DD). Optional when LastCrawledAt exists.</param>
/// <param name="DateFrom">For by-range: start of date range (YYYY-MM-DD). Required when Type is by-range.</param>
/// <param name="DateTo">For by-range: end of date range (YYYY-MM-DD). Required when Type is by-range.</param>
/// <param name="UseCache">When true, use cached external downloads (PDFs, API responses) instead of re-downloading from source.</param>
/// <param name="Reprocess">When true, skip deduplication checks to allow reprocessing already-indexed documents.</param>
public record RunCrawlerRequest(
    string Type,
    string DocumentType = "ruling",
    string? Since = null,
    string? DateFrom = null,
    string? DateTo = null,
    bool UseCache = false,
    bool Reprocess = false,
    int? MaxDocuments = null,
    int? SkipDocuments = null);
