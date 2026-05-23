using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class RulingRepository : IRulingRepository
{
    private readonly AppDbContext _context;

    public RulingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ruling?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Rulings
            .Include(r => r.Source)
            .Include(r => r.Court)
            .Include(r => r.RulingParticipations)
                .ThenInclude(rp => rp.Person)
            .Include(r => r.RulingKeywords)
                .ThenInclude(rk => rk.Keyword)
            .Include(r => r.RulingStatutes)
                .ThenInclude(rs => rs.Statute)
            .Include(r => r.OutboundCitations)
                .ThenInclude(c => c.TargetRuling)
            .Include(r => r.InboundCitations)
            .Include(r => r.ProsecutorOpinion)
            .Include(r => r.Votes)
                .ThenInclude(v => v.Participations)
                    .ThenInclude(p => p.Person)
            .Include(r => r.LegalDoctrines)
                .ThenInclude(d => d.OverruledByRuling)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Dictionary<Guid, RulingChatMetadata>> GetChatMetadataBatchAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
            return [];

        return await _context.Rulings
            .Where(r => idList.Contains(r.Id))
            .Select(r => new
            {
                r.Id,
                Meta = new RulingChatMetadata(
                    r.CaseTitle,
                    r.Summary,
                    r.Holding,
                    r.RulingDate,
                    r.JurisdictionArea,
                    r.Instance,
                    r.Court != null ? r.Court.Name : null)
            })
            .ToDictionaryAsync(x => x.Id, x => x.Meta, cancellationToken);
    }

    public async Task<Ruling?> GetByContentHashAsync(string contentHash, CancellationToken cancellationToken = default)
    {
        // Index seek on IX_Rulings_ContentHash; project only columns needed by Persister (skip LOB/wide row read).
        return await _context.Rulings
            .AsNoTracking()
            .Where(r => r.ContentHash == contentHash)
            .Select(r => new Ruling
            {
                Id = r.Id,
                ContentHash = r.ContentHash
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByContentHashAsync(string contentHash, CancellationToken cancellationToken = default)
    {
        return await _context.Rulings
            .AsNoTracking()
            .AnyAsync(r => r.ContentHash == contentHash, cancellationToken);
    }

    public async Task<bool> ExistsByExternalIdAsync(int sourceId, string externalId, CancellationToken cancellationToken = default)
    {
        return await _context.Rulings
            .AnyAsync(r => r.SourceId == sourceId && r.ExternalId == externalId, cancellationToken);
    }

    public async Task<Ruling?> FindByExternalIdAsync(int sourceId, string externalId, CancellationToken cancellationToken = default)
    {
        return await _context.Rulings
            .FirstOrDefaultAsync(r => r.SourceId == sourceId && r.ExternalId == externalId, cancellationToken);
    }

    public async Task AddAsync(Ruling ruling, CancellationToken cancellationToken = default)
    {
        await _context.Rulings.AddAsync(ruling, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ruling>> FindByCaseNumberOrExternalAliasAsync(
        string normalizedValue,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(normalizedValue))
            return [];

        var value = normalizedValue.Trim();
        return await _context.Rulings
            .AsNoTracking()
            .Where(r => r.CaseNumber == value || r.ExternalId == value)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CountFilters filters, CancellationToken cancellationToken = default)
    {
        IQueryable<Ruling> query = _context.Rulings.Where(r => r.Status == RulingStatus.Indexed);

        if (!string.IsNullOrWhiteSpace(filters.JurisdictionArea))
            query = query.Where(r => r.JurisdictionArea == filters.JurisdictionArea);

        if (!string.IsNullOrWhiteSpace(filters.Instance))
            query = query.Where(r => r.Instance == filters.Instance);

        if (!string.IsNullOrWhiteSpace(filters.CourtName))
            query = query.Where(r => r.Court != null && EF.Functions.Like(r.Court.Name, $"%{filters.CourtName}%"));

        if (filters.DateFrom.HasValue)
            query = query.Where(r => r.RulingDate >= filters.DateFrom.Value);

        if (filters.DateTo.HasValue)
            query = query.Where(r => r.RulingDate <= filters.DateTo.Value);

        if (filters.IsUnconstitutional.HasValue)
            query = query.Where(r => r.IsUnconstitutional == filters.IsUnconstitutional.Value);

        if (filters.Keywords is { Count: > 0 })
            query = query.Where(r =>
                r.RulingKeywords.Any(rk => filters.Keywords.Contains(rk.Keyword.Description)));

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetCaseTitlesByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, string>();

        return await _context.Rulings
            .Where(r => idList.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.CaseTitle ?? string.Empty, cancellationToken);
    }

    public async Task<Ruling?> FindByAnalysisIdAsync(string analysisId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(analysisId))
            return null;

        return await _context.Rulings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.AnalysisId == analysisId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, Guid>> FindRulingIdsByAnalysisIdsAsync(
        IEnumerable<string> analysisIds,
        CancellationToken cancellationToken = default)
    {
        var idSet = analysisIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToHashSet(StringComparer.Ordinal);

        if (idSet.Count == 0)
            return new Dictionary<string, Guid>(StringComparer.Ordinal);

        var rows = await _context.Rulings
            .AsNoTracking()
            .Where(r => r.AnalysisId != null && idSet.Contains(r.AnalysisId))
            .Select(r => new { AnalysisId = r.AnalysisId!, r.Id })
            .ToListAsync(cancellationToken);

        var map = new Dictionary<string, Guid>(StringComparer.Ordinal);
        foreach (var row in rows)
            map.TryAdd(row.AnalysisId, row.Id);

        return map;
    }

    public async Task<IReadOnlyList<Ruling>> GetPageForRequeueAsync(
        Guid lastId,
        int pageSize,
        bool onlyMissingOntology,
        int? sourceId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Ruling> query = _context.Rulings
            .Include(r => r.Court)
            .Include(r => r.RulingParticipations).ThenInclude(rp => rp.Person)
            .Include(r => r.RulingKeywords).ThenInclude(rk => rk.Keyword)
            .Include(r => r.RulingStatutes).ThenInclude(rs => rs.Statute)
            .Include(r => r.OutboundCitations)
            .Where(r => r.Id.CompareTo(lastId) > 0);

        if (onlyMissingOntology)
            query = query.Where(r => r.LegalBranch == null);

        if (sourceId.HasValue)
            query = query.Where(r => r.SourceId == sourceId.Value);

        return await query
            .OrderBy(r => r.Id)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ruling>> GetCsjnRulingsWithoutProsecutorOpinionAsync(
        Guid lastId,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.Rulings
            .Where(r => r.SourceId == 1
                && r.FullText != null && r.FullText != ""
                && r.ProsecutorOpinion == null
                && r.Id.CompareTo(lastId) > 0)
            .OrderBy(r => r.Id)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddProsecutorOpinionAsync(ProsecutorOpinion opinion, CancellationToken cancellationToken = default)
    {
        _context.ProsecutorOpinions.Add(opinion);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteAllProsecutorOpinionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.ExecuteSqlRawAsync(
            "DELETE FROM ProsecutorOpinions", cancellationToken);
    }

    public async Task<IReadOnlyList<Ruling>> GetRulingsWithoutDoctrineAsync(
        Guid lastId,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.Rulings
            .Where(r => r.FullText != null && r.FullText != ""
                && r.RatioDecidendi == null
                && r.Id.CompareTo(lastId) > 0)
            .OrderBy(r => r.Id)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateDoctrineFieldsAsync(
        Guid rulingId,
        string? ratioDecidendi,
        string? doctrinaLegal,
        CancellationToken cancellationToken = default)
    {
        await _context.Rulings
            .Where(r => r.Id == rulingId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.RatioDecidendi, ratioDecidendi)
                .SetProperty(r => r.DoctrinaLegal, doctrinaLegal),
                cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid rulingId, RulingStatus status, CancellationToken cancellationToken = default)
    {
        await _context.Rulings
            .Where(r => r.Id == rulingId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(r => r.Status, status),
                cancellationToken);
    }
}
