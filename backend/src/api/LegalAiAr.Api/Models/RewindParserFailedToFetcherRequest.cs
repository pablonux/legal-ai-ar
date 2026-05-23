namespace LegalAiAr.Api.Models;

/// <summary>
/// Body for POST /api/admin/jobs/{jobId}/documents/rewind-parser-failed-to-fetcher.
/// </summary>
public sealed class RewindParserFailedToFetcherRequest
{
    /// <summary>
    /// When true, only CSJN (SourceId=1) rows whose error contains "CSJN API cache miss". Default true.
    /// </summary>
    public bool OnlyCsjnCacheMiss { get; set; } = true;

    /// <summary>
    /// Used when <see cref="OnlyCsjnCacheMiss"/> is false: substring match on <c>Documents.ErrorMessage</c>.
    /// </summary>
    public string? ErrorMessageContains { get; set; }

    /// <summary>
    /// Used when <see cref="OnlyCsjnCacheMiss"/> is false: filter by source id.
    /// </summary>
    public int? SourceId { get; set; }

    /// <summary>
    /// Maximum documents to rewind in one request (1–20000). Default 5000.
    /// </summary>
    public int MaxDocuments { get; set; } = 5000;
}
