using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Statutes;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Statutes.Queries.GetStatutes;

public class GetStatutesHandler : IRequestHandler<GetStatutesQuery, StatutePageResponse>
{
    private readonly IStatuteRepository _repo;

    public GetStatutesHandler(IStatuteRepository repo) => _repo = repo;

    public async Task<StatutePageResponse> Handle(GetStatutesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repo.SearchAsync(
            request.Search,
            request.NormType,
            request.NormativeLevel,
            request.LegalBranch,
            request.IsVigente,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(s => new StatuteListItemDto(
            s.Id,
            s.Number,
            s.Name,
            s.NormType?.ToString(),
            s.NormativeLevel?.ToString(),
            s.LegalBranch?.ToString(),
            s.IssuingBody,
            s.SanctionDate,
            s.Status?.ToString(),
            s.IsVigente,
            s.RulingCount
        )).ToList();

        return new StatutePageResponse(dtos, totalCount, request.Page, request.PageSize);
    }
}
