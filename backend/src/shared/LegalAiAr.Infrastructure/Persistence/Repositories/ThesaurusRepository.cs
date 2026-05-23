using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class ThesaurusRepository : IThesaurusRepository
{
    private readonly AppDbContext _context;

    public ThesaurusRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ThesaurusTerm?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusTerms
            .FirstOrDefaultAsync(t => t.ExternalId == externalId, cancellationToken);
    }

    public async Task<ThesaurusTerm?> GetByLabelAsync(string label, CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusTerms
            .FirstOrDefaultAsync(t => t.Label == label && t.IsPreferred, cancellationToken);
    }

    public async Task<ThesaurusTerm?> ResolvePreferredTermAsync(string label, CancellationToken cancellationToken = default)
    {
        // 1. Direct match on preferred term
        var preferred = await _context.ThesaurusTerms
            .FirstOrDefaultAsync(t => t.Label == label && t.IsPreferred, cancellationToken);
        if (preferred != null) return preferred;

        // 2. Match via non-preferred synonym → follow UF relation to preferred term
        var nonPreferred = await _context.ThesaurusTerms
            .FirstOrDefaultAsync(t => t.Label == label && !t.IsPreferred, cancellationToken);
        if (nonPreferred == null) return null;

        var ufRelation = await _context.ThesaurusRelations
            .Include(r => r.SourceTerm)
            .FirstOrDefaultAsync(r => r.TargetTermId == nonPreferred.Id
                                   && r.RelationType == ThesaurusRelationType.UF, cancellationToken);

        return ufRelation?.SourceTerm;
    }

    public async Task<IReadOnlyList<ThesaurusTerm>> SearchAsync(string query, int limit = 20, CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusTerms
            .Where(t => t.IsPreferred && EF.Functions.Like(t.Label, $"%{query}%"))
            .OrderBy(t => t.Label)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ThesaurusTerm>> GetRootTermsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusTerms
            .Where(t => t.IsPreferred && t.Depth == 0)
            .OrderBy(t => t.Label)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ThesaurusTerm>> GetChildrenAsync(int termId, CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusRelations
            .Where(r => r.SourceTermId == termId && r.RelationType == ThesaurusRelationType.NT)
            .Select(r => r.TargetTerm)
            .Where(t => t.IsPreferred)
            .OrderBy(t => t.Label)
            .ToListAsync(cancellationToken);
    }

    public async Task<ThesaurusTerm?> GetByIdWithRelationsAsync(int id, CancellationToken cancellationToken = default)
    {
        var term = await _context.ThesaurusTerms
            .FirstOrDefaultAsync(t => t.Id == id && t.IsPreferred, cancellationToken);
        if (term == null) return null;

        var relations = await _context.ThesaurusRelations
            .Include(r => r.TargetTerm)
            .Where(r => r.SourceTermId == id)
            .ToListAsync(cancellationToken);

        foreach (var r in relations)
            term.RelationsAsSource.Add(r);

        return term;
    }

    public async Task<IReadOnlyList<ThesaurusTerm>> SearchWithRelationsAsync(string query, int limit = 5, CancellationToken cancellationToken = default)
    {
        var terms = await _context.ThesaurusTerms
            .Where(t => t.IsPreferred && EF.Functions.Like(t.Label, $"%{query}%"))
            .OrderByDescending(t => t.Label == query) // exact match first
            .ThenBy(t => t.Label.Length)
            .Take(limit)
            .ToListAsync(cancellationToken);

        if (terms.Count == 0) return terms;

        var termIds = terms.Select(t => t.Id).ToHashSet();

        var relations = await _context.ThesaurusRelations
            .Include(r => r.TargetTerm)
            .Where(r => termIds.Contains(r.SourceTermId))
            .ToListAsync(cancellationToken);

        var grouped = relations.GroupBy(r => r.SourceTermId);
        foreach (var g in grouped)
        {
            var term = terms.First(t => t.Id == g.Key);
            foreach (var r in g) term.RelationsAsSource.Add(r);
        }

        return terms;
    }

    public async Task<IReadOnlyList<(string Preferred, string NonPreferred)>> GetAllSynonymPairsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusRelations
            .Where(r => r.RelationType == ThesaurusRelationType.UF)
            .Select(r => new { r.SourceTerm.Label, NonPreferred = r.TargetTerm.Label })
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<(string, string)>)t.Result
                .Select(x => (x.Label, x.NonPreferred))
                .ToList(), cancellationToken);
    }

    public async Task<IReadOnlyList<int>> GetAllExternalIdsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusTerms
            .Where(t => t.IsPreferred)
            .Select(t => t.ExternalId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<int, int>> GetExternalIdToDbIdMapAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusTerms
            .Select(t => new { t.ExternalId, t.Id })
            .ToDictionaryAsync(t => t.ExternalId, t => t.Id, cancellationToken);
    }

    public async Task UpsertTermAsync(ThesaurusTerm term, CancellationToken cancellationToken = default)
    {
        var local = _context.ThesaurusTerms.Local
            .FirstOrDefault(t => t.ExternalId == term.ExternalId);

        if (local != null)
        {
            local.Label = term.Label;
            if (term.IsPreferred) local.IsPreferred = true;
            local.BranchName ??= term.BranchName;
            local.Depth = Math.Min(local.Depth, term.Depth);
            local.UpdatedAtUtc = DateTime.UtcNow;
            return;
        }

        var existing = await _context.ThesaurusTerms
            .FirstOrDefaultAsync(t => t.ExternalId == term.ExternalId, cancellationToken);

        if (existing is null)
        {
            term.CreatedAtUtc = DateTime.UtcNow;
            term.UpdatedAtUtc = DateTime.UtcNow;
            await _context.ThesaurusTerms.AddAsync(term, cancellationToken);
        }
        else
        {
            existing.Label = term.Label;
            if (term.IsPreferred) existing.IsPreferred = true;
            existing.BranchName ??= term.BranchName;
            existing.Depth = Math.Min(existing.Depth, term.Depth);
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    public async Task UpsertRelationAsync(int sourceTermId, int targetTermId, ThesaurusRelationType relationType, CancellationToken cancellationToken = default)
    {
        var exists = await _context.ThesaurusRelations
            .AnyAsync(r => r.SourceTermId == sourceTermId
                        && r.TargetTermId == targetTermId
                        && r.RelationType == relationType, cancellationToken);

        if (!exists)
        {
            await _context.ThesaurusRelations.AddAsync(new ThesaurusRelation
            {
                SourceTermId = sourceTermId,
                TargetTermId = targetTermId,
                RelationType = relationType
            }, cancellationToken);
        }
    }

    public async Task<int> GetTermCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusTerms.CountAsync(cancellationToken);
    }

    public async Task<int> GetRelationCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ThesaurusRelations.CountAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
