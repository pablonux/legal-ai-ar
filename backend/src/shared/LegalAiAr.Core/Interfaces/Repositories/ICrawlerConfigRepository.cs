using LegalAiAr.Core.Entities;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface ICrawlerConfigRepository
{
    Task<CrawlerConfig?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CrawlerConfig?> GetBySourceIdAsync(int sourceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CrawlerConfig>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates crawl status for the given source.
    /// </summary>
    Task UpdateCrawlStatusAsync(int sourceId, DateTime lastCrawledAt, string status, int documentCount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates IsEnabled for the given source.
    /// </summary>
    Task UpdateIsEnabledAsync(int sourceId, bool isEnabled, CancellationToken cancellationToken = default);
}
