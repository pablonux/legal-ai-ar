using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class CrawlerConfigRepository : ICrawlerConfigRepository
{
    private readonly AppDbContext _context;

    public CrawlerConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CrawlerConfig?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CrawlerConfigs
            .Include(c => c.Source)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<CrawlerConfig?> GetBySourceIdAsync(int sourceId, CancellationToken cancellationToken = default)
    {
        return await _context.CrawlerConfigs
            .Include(c => c.Source)
            .FirstOrDefaultAsync(c => c.SourceId == sourceId, cancellationToken);
    }

    public async Task<IReadOnlyList<CrawlerConfig>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CrawlerConfigs
            .Include(c => c.Source)
            .OrderBy(c => c.SourceId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateCrawlStatusAsync(int sourceId, DateTime lastCrawledAt, string status, int documentCount, CancellationToken cancellationToken = default)
    {
        var config = await _context.CrawlerConfigs.FirstOrDefaultAsync(c => c.SourceId == sourceId, cancellationToken);
        if (config is null)
            return;

        config.LastCrawledAt = lastCrawledAt;
        config.LastCrawledStatus = status;
        config.LastDocumentCount = documentCount;
        config.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateIsEnabledAsync(int sourceId, bool isEnabled, CancellationToken cancellationToken = default)
    {
        var config = await _context.CrawlerConfigs.FirstOrDefaultAsync(c => c.SourceId == sourceId, cancellationToken);
        if (config is null)
            return;

        config.IsEnabled = isEnabled;
        config.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
