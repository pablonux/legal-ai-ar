using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Catalogs.Queries.GetCourts;

public class GetCourtsHandler : IRequestHandler<GetCourtsQuery, IReadOnlyList<CourtListItemDto>>
{
    private readonly ICourtRepository _repo;

    public GetCourtsHandler(ICourtRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<CourtListItemDto>> Handle(
        GetCourtsQuery request, CancellationToken cancellationToken)
    {
        var courts = await _repo.SearchAsync(
            request.Search, request.JurisdictionArea, request.Instance,
            Math.Clamp(request.Limit, 1, 100), cancellationToken);

        return courts.Select(c => new CourtListItemDto(
            c.Id, c.Name, c.JurisdictionArea, c.Territory, c.Instance, c.RulingCount
        )).ToList();
    }
}
