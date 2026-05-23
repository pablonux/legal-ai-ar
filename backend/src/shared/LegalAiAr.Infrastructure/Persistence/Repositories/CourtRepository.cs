using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class CourtRepository : ICourtRepository
{
    private readonly AppDbContext _context;
    private readonly EntityCacheService _cache;

    public CourtRepository(AppDbContext context, EntityCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Court?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Courts
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Court> GetOrCreateAsync(string name, string jurisdictionArea, string territory, string instance, CancellationToken cancellationToken = default)
    {
        // Do not Attach(cached) from warm-up: AsNoTracking() materialization can fix up ParentCourt/ChildCourts
        // and Attach traverses the whole graph, causing TraverseGraph failures or duplicate key tracking.
        if (_cache.IsWarmedUp && _cache.TryGetCourtByName(name, out var cached) && cached is { Id: > 0 })
        {
            var tracked = _context.Courts.Find(cached.Id);
            if (tracked != null)
                return tracked;
        }

        var court = await _context.Courts
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);

        if (court != null)
        {
            _cache.SetCourt(court);
            return court;
        }

        court = new Court
        {
            Name = name,
            JurisdictionArea = jurisdictionArea,
            Territory = territory,
            Instance = instance
        };
        await _context.Courts.AddAsync(court, cancellationToken);
        _cache.SetCourt(court);
        return court;
    }

    public async Task<IReadOnlyList<Court>> ListAsync(
        string? jurisdictionArea = null,
        string? instance = null,
        int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Court> query = _context.Courts;

        if (!string.IsNullOrWhiteSpace(jurisdictionArea))
            query = query.Where(c => c.JurisdictionArea == jurisdictionArea);

        if (!string.IsNullOrWhiteSpace(instance))
            query = query.Where(c => c.Instance == instance);

        return await query
            .OrderBy(c => c.Name)
            .Take(maxResults)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CourtWithCount>> SearchAsync(
        string? query = null,
        string? jurisdictionArea = null,
        string? instance = null,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Court> q = _context.Courts;

        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(c => EF.Functions.Like(c.Name, $"%{query}%"));
        if (!string.IsNullOrWhiteSpace(jurisdictionArea))
            q = q.Where(c => c.JurisdictionArea == jurisdictionArea);
        if (!string.IsNullOrWhiteSpace(instance))
            q = q.Where(c => c.Instance == instance);

        return await q
            .OrderByDescending(c => c.Rulings.Count)
            .Take(limit)
            .Select(c => new CourtWithCount(
                c.Id, c.Name, c.JurisdictionArea, c.Territory, c.Instance,
                c.Rulings.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<CourtDetail?> GetByIdWithStatsAsync(int id, CancellationToken cancellationToken = default)
    {
        var court = await _context.Courts
            .Include(c => c.ParentCourt)
            .Include(c => c.ChildCourts)
            .Include(c => c.JudicialOffices)
                .ThenInclude(o => o.Person)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (court == null) return null;

        var rulingCount = await _context.Rulings
            .CountAsync(r => r.CourtId == id, cancellationToken);

        var topPersons = await _context.Persons
            .Where(p => p.RulingParticipations.Any(rp => rp.Ruling.CourtId == id))
            .OrderByDescending(p => p.RulingParticipations.Count(rp => rp.Ruling.CourtId == id))
            .Take(20)
            .Select(p => new PersonWithCount(
                p.Id, p.FirstName ?? "", p.LastName ?? "",
                p.RulingParticipations.Count(rp => rp.Ruling.CourtId == id)))
            .ToListAsync(cancellationToken);

        return new CourtDetail(court, rulingCount, topPersons);
    }
}
