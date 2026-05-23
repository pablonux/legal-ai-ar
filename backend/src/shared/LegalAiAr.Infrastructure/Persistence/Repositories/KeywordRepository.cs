using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class KeywordRepository : IKeywordRepository
{
    private readonly AppDbContext _context;
    private readonly EntityCacheService _cache;

    public KeywordRepository(AppDbContext context, EntityCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Keyword?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Keywords
            .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
    }

    public async Task<Keyword> GetOrCreateAsync(int? externalCode, string description, CancellationToken cancellationToken = default)
    {
        var key = new KeywordLookupKey(externalCode, description);
        var map = await GetOrCreateBatchAsync(new[] { key }, cancellationToken);
        return map[key];
    }

    public async Task<IReadOnlyDictionary<KeywordLookupKey, Keyword>> GetOrCreateBatchAsync(
        IReadOnlyCollection<KeywordLookupKey> keys,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<KeywordLookupKey, Keyword>();
        if (keys.Count == 0)
            return result;

        var distinct = keys.Distinct().ToList();

        foreach (var key in distinct)
        {
            if (TryResolveFromWarmCache(key, out var tracked))
                result[key] = tracked;
        }

        var needDb = distinct.Where(k => !result.ContainsKey(k)).ToList();
        if (needDb.Count == 0)
            return result;

        var codes = needDb.Where(k => k.ExternalCode.HasValue).Select(k => k.ExternalCode!.Value).Distinct().ToList();
        var descs = needDb.Select(k => k.Description).Distinct().ToList();

        var pool = await _context.Keywords
            .Where(k =>
                (k.ExternalCode != null && codes.Contains(k.ExternalCode.Value))
                || descs.Contains(k.Description))
            .ToListAsync(cancellationToken);

        foreach (var key in needDb)
        {
            var kw = FindInPool(pool, key);
            if (kw == null)
                continue;
            _cache.SetKeyword(kw);
            result[key] = kw;
        }

        var needCreate = needDb.Where(k => !result.ContainsKey(k)).ToList();
        foreach (var key in needCreate)
        {
            var existing = FindInPool(pool, key);
            if (existing != null)
            {
                result[key] = existing;
                continue;
            }

            var created = new Keyword { ExternalCode = key.ExternalCode, Description = key.Description };
            await _context.Keywords.AddAsync(created, cancellationToken);
            pool.Add(created);
            _cache.SetKeyword(created);
            result[key] = created;
        }

        return result;
    }

    private bool TryResolveFromWarmCache(KeywordLookupKey key, out Keyword tracked)
    {
        tracked = null!;
        if (!_cache.IsWarmedUp)
            return false;

        Keyword? cached = null;
        if (key.ExternalCode.HasValue)
            _cache.TryGetKeywordByExternalCode(key.ExternalCode.Value, out cached);
        cached ??= _cache.TryGetKeywordByDescription(key.Description, out var byDesc) ? byDesc : null;

        if (cached is not { Id: > 0 })
            return false;

        tracked = GetTrackedFromCachedKeyword(cached);
        return true;
    }

    private Keyword GetTrackedFromCachedKeyword(Keyword cached)
    {
        var local = _context.Keywords.Local.FirstOrDefault(k => k.Id == cached.Id);
        if (local != null)
            return local;

        var attached = new Keyword
        {
            Id = cached.Id,
            ExternalCode = cached.ExternalCode,
            Description = cached.Description,
            ThesaurusTermId = cached.ThesaurusTermId,
        };
        _context.Attach(attached);
        return attached;
    }

    private static Keyword? FindInPool(List<Keyword> pool, KeywordLookupKey key)
    {
        if (key.ExternalCode.HasValue)
        {
            foreach (var x in pool)
            {
                if (x.ExternalCode == key.ExternalCode.Value)
                    return x;
            }
        }

        foreach (var x in pool)
        {
            if (x.Description == key.Description)
                return x;
        }

        return null;
    }
}
