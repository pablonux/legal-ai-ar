using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.DTOs;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Proceedings.Queries;

public class GetAppealChainHandler : IRequestHandler<GetAppealChainQuery, AppealChainDto?>
{
    private readonly IJudicialProceedingRepository _repo;

    public GetAppealChainHandler(IJudicialProceedingRepository repo) => _repo = repo;

    public async Task<AppealChainDto?> Handle(GetAppealChainQuery request, CancellationToken ct)
    {
        var result = await _repo.GetAppealChainAsync(request.ProceedingId, ct);
        if (result is null) return null;

        var (proc, remedies) = result.Value;

        var rulings = proc.Rulings
            .OrderBy(r => r.RulingDate)
            .ToList();

        var nodes = rulings.Select(r =>
        {
            var fromHere = remedies
                .Where(rem => rem.AppealedRulingId == r.Id)
                .Select(rem => new ProceduralRemedyDto(
                    rem.Id,
                    rem.RemedyType.ToString(),
                    rem.FilingDate,
                    rem.ResolutionDate,
                    rem.Outcome,
                    rem.ResolvingRulingId,
                    rem.ResolvingRuling?.CaseTitle,
                    rem.AppealedRulingId,
                    rem.AppealedRuling?.CaseTitle,
                    rem.CourtAQuo?.Name,
                    rem.CourtAdQuem?.Name))
                .ToList();

            return new AppealChainNodeDto(
                r.Id, r.CaseTitle, r.RulingDate, r.Instance,
                r.Court?.Name ?? "Desconocido",
                fromHere);
        }).ToList();

        return new AppealChainDto(proc.Id, proc.CaseNumber, proc.DisplayName, nodes);
    }
}
