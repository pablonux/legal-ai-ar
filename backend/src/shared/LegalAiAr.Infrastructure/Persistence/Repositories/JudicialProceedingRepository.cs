using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

public class JudicialProceedingRepository : IJudicialProceedingRepository
{
    private readonly AppDbContext _db;

    public JudicialProceedingRepository(AppDbContext db) => _db = db;

    public async Task<(IReadOnlyList<JudicialProceeding> Items, int TotalCount)> SearchAsync(
        string? search,
        ProcessType? processType,
        LegalBranch? legalBranch,
        int? courtId,
        ProcessStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var q = _db.JudicialProceedings.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => EF.Functions.Like(p.CaseNumber, $"%{search}%")
                          || (p.DisplayName != null && EF.Functions.Like(p.DisplayName, $"%{search}%")));
        if (processType.HasValue) q = q.Where(p => p.ProcessType == processType.Value);
        if (legalBranch.HasValue) q = q.Where(p => p.LegalBranch == legalBranch.Value);
        if (courtId.HasValue) q = q.Where(p => p.CourtId == courtId.Value);
        if (status.HasValue) q = q.Where(p => p.Status == status.Value);

        var totalCount = await q.CountAsync(cancellationToken);

        var items = await q
            .Include(p => p.Court)
            .OrderByDescending(p => p.LastRulingDate)
            .ThenByDescending(p => p.RulingCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<JudicialProceeding?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.JudicialProceedings
            .AsNoTracking()
            .Include(p => p.Court)
            .Include(p => p.Rulings).ThenInclude(r => r.Court)
            .Include(p => p.Parties).ThenInclude(pp => pp.Person)
            .Include(p => p.LegalRepresentations).ThenInclude(lr => lr.LawyerPerson)
            .Include(p => p.LegalRepresentations).ThenInclude(lr => lr.PartyPerson)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<JudicialProceeding?> GetByRulingIdAsync(Guid rulingId, CancellationToken cancellationToken = default)
    {
        var ruling = await _db.Rulings
            .AsNoTracking()
            .Where(r => r.Id == rulingId && r.JudicialProceedingId != null)
            .Select(r => r.JudicialProceedingId)
            .FirstOrDefaultAsync(cancellationToken);

        if (ruling is null)
            return null;

        return await _db.JudicialProceedings
            .AsNoTracking()
            .Include(p => p.Rulings)
                .ThenInclude(r => r.Court)
            .FirstOrDefaultAsync(p => p.Id == ruling.Value, cancellationToken);
    }

    public async Task<JudicialProceeding?> FindByCaseNumberAsync(
        string caseNumber, string? jurisdictionArea, CancellationToken cancellationToken = default)
    {
        return await _db.JudicialProceedings
            .FirstOrDefaultAsync(p => p.CaseNumber == caseNumber && p.JurisdictionArea == jurisdictionArea,
                cancellationToken);
    }

    public async Task AddAsync(JudicialProceeding proceeding, CancellationToken cancellationToken = default)
    {
        await _db.JudicialProceedings.AddAsync(proceeding, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<(int ProceedingsCreated, int RulingsLinked)> LinkUnlinkedRulingsAsync(CancellationToken cancellationToken = default)
    {
        var groups = await _db.Rulings
            .Where(r => r.CaseNumber != null && r.CaseNumber != "" && r.JudicialProceedingId == null)
            .GroupBy(r => new { r.CaseNumber, r.JurisdictionArea })
            .Where(g => g.Count() >= 1)
            .Select(g => new
            {
                g.Key.CaseNumber,
                g.Key.JurisdictionArea,
                RulingIds = g.Select(r => r.Id).ToList(),
                DisplayName = g.OrderByDescending(r => r.RulingDate).Select(r => r.CaseTitle).First(),
                FirstDate = g.Min(r => r.RulingDate),
                LastDate = g.Max(r => r.RulingDate),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        int created = 0, linked = 0;

        foreach (var group in groups)
        {
            var existing = await _db.JudicialProceedings
                .FirstOrDefaultAsync(p => p.CaseNumber == group.CaseNumber
                    && p.JurisdictionArea == group.JurisdictionArea, cancellationToken);

            int proceedingId;
            if (existing is not null)
            {
                proceedingId = existing.Id;
                existing.RulingCount += group.Count;
                if (existing.FirstRulingDate is null || group.FirstDate < existing.FirstRulingDate)
                    existing.FirstRulingDate = group.FirstDate;
                if (existing.LastRulingDate is null || group.LastDate > existing.LastRulingDate)
                    existing.LastRulingDate = group.LastDate;
            }
            else
            {
                var proceeding = new JudicialProceeding
                {
                    CaseNumber = group.CaseNumber!,
                    DisplayName = group.DisplayName,
                    JurisdictionArea = group.JurisdictionArea,
                    RulingCount = group.Count,
                    FirstRulingDate = group.FirstDate,
                    LastRulingDate = group.LastDate,
                    CreatedAt = DateTime.UtcNow
                };
                _db.JudicialProceedings.Add(proceeding);
                await _db.SaveChangesAsync(cancellationToken);
                proceedingId = proceeding.Id;
                created++;
            }

            await _db.Rulings
                .Where(r => group.RulingIds.Contains(r.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.JudicialProceedingId, proceedingId), cancellationToken);

            linked += group.Count;
        }

        return (created, linked);
    }

    public async Task<int> BackfillProceedingFieldsAsync(CancellationToken cancellationToken = default)
    {
        var proceedings = await _db.JudicialProceedings
            .Include(p => p.Rulings)
            .Where(p => p.Rulings.Any() &&
                (p.ProcessType == null || p.CourtId == null || p.LegalBranch == null || p.Status == null))
            .ToListAsync(cancellationToken);

        var updated = 0;

        foreach (var proc in proceedings)
        {
            var changed = false;

            if (proc.CourtId is null)
            {
                var courtId = proc.Rulings
                    .OrderBy(r => r.RulingDate)
                    .Select(r => (int?)r.CourtId)
                    .FirstOrDefault();
                if (courtId is not null)
                {
                    proc.CourtId = courtId;
                    changed = true;
                }
            }

            if (proc.LegalBranch is null)
            {
                var predominant = proc.Rulings
                    .Where(r => r.LegalBranch is not null)
                    .GroupBy(r => r.LegalBranch!.Value)
                    .OrderByDescending(g => g.Count())
                    .Select(g => (LegalBranch?)g.Key)
                    .FirstOrDefault();
                if (predominant is not null)
                {
                    proc.LegalBranch = predominant;
                    changed = true;
                }
            }

            if (proc.ProcessType is null && proc.LegalBranch is not null)
            {
                proc.ProcessType = InferProcessType(proc.LegalBranch.Value);
                changed = true;
            }

            if (proc.Status is null && proc.Rulings.Any())
            {
                proc.Status = ProcessStatus.ConSentencia;
                changed = true;
            }

            if (changed) updated++;
        }

        if (updated > 0)
            await _db.SaveChangesAsync(cancellationToken);

        return updated;
    }

    public async Task<IReadOnlyList<JudicialProceeding>> GetProceedingsWithoutPartiesAsync(
        int lastId, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _db.JudicialProceedings
            .Where(p => p.Id > lastId && !p.Parties.Any())
            .OrderBy(p => p.Id)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddPartyIfNotExistsAsync(
        int proceedingId, int personId, PartyRole role, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Set<ProceedingParty>()
            .AnyAsync(pp => pp.JudicialProceedingId == proceedingId
                && pp.PersonId == personId
                && pp.Role == role, cancellationToken);

        if (!exists)
        {
            _db.Set<ProceedingParty>().Add(new ProceedingParty
            {
                JudicialProceedingId = proceedingId,
                PersonId = personId,
                Role = role,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<(JudicialProceeding Proceeding, IReadOnlyList<ProceduralRemedy> Remedies)?> GetAppealChainAsync(
        int proceedingId, CancellationToken ct = default)
    {
        var proc = await _db.JudicialProceedings
            .AsNoTracking()
            .Include(p => p.Rulings).ThenInclude(r => r.Court)
            .FirstOrDefaultAsync(p => p.Id == proceedingId, ct);

        if (proc is null) return null;

        var rulingIds = proc.Rulings.Select(r => r.Id).ToHashSet();

        var remedies = await _db.ProceduralRemedies
            .AsNoTracking()
            .Include(r => r.CourtAQuo)
            .Include(r => r.CourtAdQuem)
            .Include(r => r.ResolvingRuling)
            .Include(r => r.AppealedRuling)
            .Where(r => r.JudicialProceedingId == proceedingId
                || (r.AppealedRulingId != null && rulingIds.Contains(r.AppealedRulingId.Value))
                || (r.ResolvingRulingId != null && rulingIds.Contains(r.ResolvingRulingId.Value)))
            .ToListAsync(ct);

        return (proc, remedies);
    }

    private static ProcessType? InferProcessType(LegalBranch branch) => branch switch
    {
        LegalBranch.PRIV_CIVIL or LegalBranch.PRIV_COM or LegalBranch.PRIV_SEG
            or LegalBranch.PRIV_PI => ProcessType.Civil,
        LegalBranch.PUB_PENAL or LegalBranch.PUB_PROC_PEN or LegalBranch.DIG_CYBER
            => ProcessType.Penal,
        LegalBranch.PRIV_LAB or LegalBranch.PRIV_LAB_COL => ProcessType.Laboral,
        LegalBranch.PUB_ADMIN or LegalBranch.PUB_TRIB => ProcessType.ContenciosoAdministrativo,
        LegalBranch.SOC_FAM or LegalBranch.SOC_NINEZ => ProcessType.Familia,
        LegalBranch.PUB_CONST => ProcessType.Constitucional,
        _ => null
    };
}
