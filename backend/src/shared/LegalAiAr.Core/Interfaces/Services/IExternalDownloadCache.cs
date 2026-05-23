namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Cache for external HTTP downloads (PDFs, API responses). Backed by Azure Blob Storage
/// under a dedicated prefix, keyed by source + document/analysis ID + endpoint.
/// Used to avoid re-downloading from external sources when re-running pipeline jobs.
/// </summary>
public interface IExternalDownloadCache
{
    /// <summary>
    /// Returns cached content for the given key, or null if not cached.
    /// </summary>
    Task<byte[]?> GetAsync(string cacheKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores content in the cache. Overwrites if the key already exists.
    /// </summary>
    Task SetAsync(string cacheKey, byte[] content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a cache entry exists for the given key.
    /// </summary>
    Task<bool> ExistsAsync(string cacheKey, CancellationToken cancellationToken = default);
}
