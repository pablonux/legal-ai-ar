using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class CitationRepository : ICitationRepository
{
    private readonly AppDbContext _context;

    public CitationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Citation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Citations
            .Include(c => c.SourceRuling)
            .Include(c => c.TargetRuling)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Citation citation, CancellationToken cancellationToken = default)
    {
        await _context.Citations.AddAsync(citation, cancellationToken);
    }

    public async Task<IReadOnlyList<Citation>> GetPendingByExternalAliasMatchAsync(
        IReadOnlyList<string> normalizedAliases,
        CancellationToken cancellationToken = default)
    {
        if (normalizedAliases.Count == 0)
            return [];

        return await _context.Citations
            .AsNoTracking()
            .Where(c => c.TargetRulingId == null &&
                normalizedAliases.Contains(c.ExternalAlias.Trim().ToLower()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Citation>> GetPendingOutboundBySourceRulingAsync(
        Guid sourceRulingId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Citations
            .AsNoTracking()
            .Where(c => c.SourceRulingId == sourceRulingId && c.TargetRulingId == null)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateTargetRulingIdAsync(int citationId, Guid targetRulingId, CancellationToken cancellationToken = default)
    {
        var citation = await _context.Citations.FindAsync([citationId], cancellationToken);
        if (citation != null)
            citation.TargetRulingId = targetRulingId;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Citation>> GetUnresolvedCitationsAsync(
        int lastId, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Citations
            .Where(c => c.TargetRulingId == null && c.Id > lastId)
            .OrderBy(c => c.Id)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid?> FindRulingByVolumenPageAsync(
        string volume, string page, CancellationToken cancellationToken = default)
    {
        var rulingIds = await _context.Set<Sumario>()
            .Where(s => s.Volume == volume && s.Page == page)
            .Select(s => s.RulingId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return rulingIds.Count == 1 ? rulingIds[0] : null;
    }

    /// <inheritdoc />
    public async Task<int> LinkStatuteToRulingsAsync(
        int statuteId, string number, string name, CancellationToken cancellationToken = default)
    {
        var linked = await _context.Database.ExecuteSqlRawAsync("""
            UPDATE c
            SET c.CitedStatuteId = {0}
            FROM Citations c
            WHERE c.CitedStatuteId IS NULL
              AND (
                c.CitationText LIKE '%' + {1} + '%'
                OR c.ExternalAlias LIKE '%' + {1} + '%'
                OR c.CitationText LIKE '%' + {2} + '%'
              )
            """,
            [statuteId, number, name],
            cancellationToken);

        return linked;
    }
}
