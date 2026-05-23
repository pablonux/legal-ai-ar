using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Thesaurus;

public sealed class KeywordNormalizationService : IKeywordNormalizationService
{
    private readonly IThesaurusRepository _thesaurusRepo;
    private readonly AppDbContext _context;
    private readonly EntityCacheService _entityCache;
    private readonly ILogger<KeywordNormalizationService> _logger;

    public KeywordNormalizationService(
        IThesaurusRepository thesaurusRepo,
        AppDbContext context,
        EntityCacheService entityCache,
        ILogger<KeywordNormalizationService> logger)
    {
        _thesaurusRepo = thesaurusRepo;
        _context = context;
        _entityCache = entityCache;
        _logger = logger;
    }

    public async Task<int?> ResolveAsync(string keywordDescription, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keywordDescription)) return null;

        var label = keywordDescription.Trim();

        if (_entityCache.TryResolvePreferredThesaurusTermId(label, out var cachedPreferredId))
            return cachedPreferredId;

        var term = await _thesaurusRepo.ResolvePreferredTermAsync(label, cancellationToken);
        return term?.Id;
    }

    public async Task<(int Matched, int Total)> NormalizeAllAsync(Action<string>? onProgress = null, CancellationToken cancellationToken = default)
    {
        var unlinked = await _context.Keywords
            .Where(k => k.ThesaurusTermId == null)
            .ToListAsync(cancellationToken);

        onProgress?.Invoke($"Found {unlinked.Count} unlinked keywords to normalize.");

        var matched = 0;
        var processed = 0;

        foreach (var keyword in unlinked)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var termId = await ResolveAsync(keyword.Description, cancellationToken);
            if (termId.HasValue)
            {
                keyword.ThesaurusTermId = termId.Value;
                matched++;
            }

            processed++;
            if (processed % 100 == 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
                onProgress?.Invoke($"  {processed}/{unlinked.Count} processed ({matched} matched)...");
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Keyword normalization complete: {Matched}/{Total} linked to thesaurus",
            matched, unlinked.Count);

        return (matched, unlinked.Count);
    }
}
