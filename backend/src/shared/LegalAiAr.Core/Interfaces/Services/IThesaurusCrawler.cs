namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Crawls an external thesaurus source and persists terms and relations to the repository.
/// </summary>
public interface IThesaurusCrawler
{
    /// <summary>
    /// Full crawl: fetches all terms and their relationships, upserting into the repository.
    /// Reports progress via <paramref name="onProgress"/>.
    /// </summary>
    Task CrawlAsync(Action<string>? onProgress = null, CancellationToken cancellationToken = default);
}
