using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline.Strategies;

/// <summary>
/// Downloads content (PDF/HTML) for a discovered document, computes content hash,
/// and uploads to Blob Storage.
/// </summary>
public interface IFetchStrategy
{
    /// <summary>
    /// Fetches document content, computes SHA-256 hash, and returns the result.
    /// The implementation should handle download cache (read/write-through).
    /// </summary>
    Task<FetchedDocument> FetchAsync(
        FetcherMessage message,
        bool useCache = false,
        CancellationToken cancellationToken = default);
}
