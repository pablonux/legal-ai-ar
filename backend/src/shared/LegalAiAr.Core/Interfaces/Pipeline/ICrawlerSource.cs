using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Contract for crawler sources (CSJN, SAIJ, PJN, SCBA). Each implementation discovers documents
/// and provides PDF content with hash for deduplication and Blob upload.
/// </summary>
public interface ICrawlerSource
{
    /// <summary>
    /// Total results reported by the source site search after the last call to <see cref="DiscoverAsync"/>.
    /// Null when the source does not provide this information or discovery has not run yet.
    /// </summary>
    int? LastTotalSearchResults { get; }

    /// <summary>
    /// Runs a search for the given range and returns only the total count from the page (no pagination).
    /// Used for pre-flight split resolution. Throws <see cref="CsjnResultLimitExceededException"/> when the site returns an error.
    /// </summary>
    /// <param name="message">Crawl message with DateFrom/DateTo (by-range).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total result count, or null if not supported.</returns>
    Task<int?> GetTotalForRangeAsync(CrawlerMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers documents for the given crawl message. Yields pages (batches) of documents as they are fetched.
    /// Each batch corresponds to one page from the source; DocumentsDiscovered is updated per batch.
    /// </summary>
    /// <param name="message">Crawl message with source ID, type (incremental/full) and optional since date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async sequence of document batches (one per fetched page).</returns>
    IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverAsync(
        CrawlerMessage message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the PDF for the given document and returns its content hash and raw bytes.
    /// The hash is used for deduplication; the bytes are used for Blob Storage upload.
    /// Returns both to avoid downloading the PDF twice.
    /// </summary>
    /// <param name="documentId">External document ID in the source.</param>
    /// <param name="analysisId">Source-specific analysis ID. Null for sources that do not use it (e.g. SAIJ).</param>
    /// <param name="useCache">When true, check download cache before HTTP call. Always writes to cache (write-through).</param>
    /// <param name="httpRequestTimeoutSeconds">When set, use a dedicated HTTP client with this timeout (seconds) for this download only; null uses the worker default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Content hash (SHA-256) and PDF bytes. Throws if download fails.</returns>
    Task<CrawlerDocumentContent> GetContentHashAsync(
        string documentId,
        string? analysisId,
        bool useCache = false,
        int? httpRequestTimeoutSeconds = null,
        CancellationToken cancellationToken = default);
}
