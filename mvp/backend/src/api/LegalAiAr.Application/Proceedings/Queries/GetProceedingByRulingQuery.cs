using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Models;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Proceedings.Queries;

public record GetProceedingByRulingQuery(Guid RulingId) : IRequest<ProceedingResponse?>;

public class GetProceedingByRulingHandler : IRequestHandler<GetProceedingByRulingQuery, ProceedingResponse?>
{
    private readonly IJudicialProceedingRepository _repo;

    public GetProceedingByRulingHandler(IJudicialProceedingRepository repo) => _repo = repo;

    public async Task<ProceedingResponse?> Handle(GetProceedingByRulingQuery request, CancellationToken cancellationToken)
    {
        var proceeding = await _repo.GetByRulingIdAsync(request.RulingId, cancellationToken);
        if (proceeding is null)
            return null;

        var rulings = proceeding.Rulings
            .OrderBy(r => r.Court?.InstanceLevel ?? 99)
            .ThenBy(r => r.RulingDate)
            .Select(r => new ProceedingRulingDto(
                r.Id,
                r.CaseTitle,
                r.RulingDate,
                r.Court?.Name ?? "N/A",
                r.Court?.InstanceLevel,
                r.RulingDirection,
                r.Id == request.RulingId))
            .ToList();

        return new ProceedingResponse(
            proceeding.Id,
            proceeding.CaseNumber,
            proceeding.DisplayName,
            proceeding.JurisdictionArea,
            rulings);
    }
}
