using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class StatuteRepository : IStatuteRepository
{
    private readonly AppDbContext _context;
    private readonly EntityCacheService _cache;

    public StatuteRepository(AppDbContext context, EntityCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Statute?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Statutes
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Statute> GetOrCreateAsync(string number, string name, CancellationToken cancellationToken = default)
    {
        var map = await GetOrCreateBatchAsync(new[] { (number, name) }, cancellationToken);
        var statute = map[number];
        if (statute.Name != name)
            statute.Name = name;
        return statute;
    }

    public async Task<IReadOnlyDictionary<string, Statute>> GetOrCreateBatchAsync(
        IReadOnlyCollection<(string Number, string Name)> pairs,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, Statute>(StringComparer.Ordinal);
        if (pairs.Count == 0)
            return result;

        var distinctOrdered = new List<(string Number, string Name)>();
        var seenNumbers = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (number, name) in pairs)
        {
            if (string.IsNullOrWhiteSpace(number))
                continue;
            if (seenNumbers.Add(number))
                distinctOrdered.Add((number, name));
        }

        if (distinctOrdered.Count == 0)
            return result;

        foreach (var (number, _) in distinctOrdered)
        {
            if (result.ContainsKey(number))
                continue;

            if (_cache.IsWarmedUp && _cache.TryGetStatuteByNumber(number, out var cached) && cached is { Id: > 0 })
            {
                var tracked = _context.Statutes.Find(cached.Id);
                if (tracked != null)
                    result[number] = tracked;
            }
        }

        var needDb = distinctOrdered.Where(p => !result.ContainsKey(p.Number)).ToList();
        if (needDb.Count == 0)
            return result;

        var numbers = needDb.Select(p => p.Number).Distinct(StringComparer.Ordinal).ToList();
        var pool = await _context.Statutes
            .Where(s => numbers.Contains(s.Number))
            .ToListAsync(cancellationToken);

        foreach (var (number, name) in needDb)
        {
            var match = pool.FirstOrDefault(s => s.Number == number);
            if (match is null)
                continue;
            if (match.Name != name)
                match.Name = name;
            _cache.SetStatute(match);
            result[number] = match;
        }

        foreach (var (number, name) in needDb)
        {
            if (result.ContainsKey(number))
                continue;

            var statute = new Statute { Number = number, Name = name };
            await _context.Statutes.AddAsync(statute, cancellationToken);
            pool.Add(statute);
            _cache.SetStatute(statute);
            result[number] = statute;
        }

        return result;
    }

    public async Task<(IReadOnlyList<StatuteWithCount> Items, int TotalCount)> SearchAsync(
        string? search,
        NormType? normType,
        NormativeLevel? normativeLevel,
        LegalBranch? legalBranch,
        bool? isVigente,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Statute> q = _context.Statutes;

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(s => EF.Functions.Like(s.Name, $"%{search}%") || EF.Functions.Like(s.Number, $"%{search}%"));
        if (normType.HasValue)
            q = q.Where(s => s.NormType == normType.Value);
        if (normativeLevel.HasValue)
            q = q.Where(s => s.NormativeLevel == normativeLevel.Value);
        if (legalBranch.HasValue)
            q = q.Where(s => s.LegalBranch == legalBranch.Value);
        if (isVigente.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            q = isVigente.Value
                ? q.Where(s => s.EffectiveTo == null || s.EffectiveTo > today)
                : q.Where(s => s.EffectiveTo != null && s.EffectiveTo <= today);
        }

        var totalCount = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(s => s.RulingStatutes.Count)
            .ThenBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new StatuteWithCount(
                s.Id, s.Number, s.Name,
                s.NormType, s.NormativeLevel, s.LegalBranch,
                s.IssuingBody, s.SanctionDate, s.EffectiveTo,
                s.Status, s.RulingStatutes.Count))
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Statute?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Statutes
            .Include(s => s.RulingStatutes)
                .ThenInclude(rs => rs.Ruling)
                    .ThenInclude(r => r.Court)
            .Include(s => s.OutboundNormRelations)
                .ThenInclude(nr => nr.TargetStatute)
            .Include(s => s.InboundNormRelations)
                .ThenInclude(nr => nr.SourceStatute)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Dictionary<NormativeLevel, int>> GetCountsByNormativeLevelAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Statutes
            .Where(s => s.NormativeLevel != null)
            .GroupBy(s => s.NormativeLevel!.Value)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<Dictionary<NormativeLevel, int>> GetVigenteCountsByNormativeLevelAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _context.Statutes
            .Where(s => s.NormativeLevel != null &&
                (s.Status == StatuteStatus.Vigente
                 || (s.Status == null && (s.EffectiveTo == null || s.EffectiveTo > today))))
            .GroupBy(s => s.NormativeLevel!.Value)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<IReadOnlyList<StatuteRulingResult>> FindRulingsByStatuteAsync(
        string statuteName,
        string? statuteNumber = null,
        string? articles = null,
        int topK = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _context.RulingStatutes
            .Include(rs => rs.Statute)
            .Include(rs => rs.Ruling)
                .ThenInclude(r => r.Court)
            .Where(rs => EF.Functions.Like(rs.Statute.Name, $"%{statuteName}%"));

        if (!string.IsNullOrWhiteSpace(statuteNumber))
            query = query.Where(rs => rs.Statute.Number == statuteNumber);

        if (!string.IsNullOrWhiteSpace(articles))
            query = query.Where(rs => rs.Articles != null && EF.Functions.Like(rs.Articles, $"%{articles}%"));

        return await query
            .OrderByDescending(rs => rs.Ruling.RulingDate)
            .Take(topK)
            .Select(rs => new StatuteRulingResult(
                rs.Ruling.Id,
                rs.Ruling.CaseTitle,
                rs.Ruling.RulingDate,
                rs.Ruling.Court != null ? rs.Ruling.Court.Name : null,
                rs.Ruling.Summary,
                rs.Statute.Name,
                rs.Statute.Number,
                rs.Articles))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Statute>> GetUnclassifiedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Statutes
            .Where(s => s.NormType == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> BackfillLegalBranchFromRulingsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.ExecuteSqlRawAsync("""
            WITH latest AS (
                SELECT
                    rs.StatuteId,
                    r.LegalBranch,
                    ROW_NUMBER() OVER (PARTITION BY rs.StatuteId ORDER BY r.RulingDate DESC, r.Id DESC) AS rn
                FROM RulingStatutes rs
                INNER JOIN Rulings r ON r.Id = rs.RulingId
                WHERE r.LegalBranch IS NOT NULL
            )
            UPDATE s
            SET s.LegalBranch = latest.LegalBranch
            FROM Statutes s
            INNER JOIN latest ON latest.StatuteId = s.Id AND latest.rn = 1
            WHERE s.LegalBranch IS NULL
            """, cancellationToken);
    }

    public async Task<Statute?> FindBySaijIdAsync(string saijId, CancellationToken cancellationToken = default)
    {
        return await _context.Statutes
            .FirstOrDefaultAsync(s => s.SaijId == saijId, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
