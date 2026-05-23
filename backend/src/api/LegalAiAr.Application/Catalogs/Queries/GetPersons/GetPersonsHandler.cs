using LegalAiAr.Application.Catalogs.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Catalogs.Queries.GetPersons;

public class GetPersonsHandler : IRequestHandler<GetPersonsQuery, IReadOnlyList<PersonListItemDto>>
{
    private readonly IPersonRepository _repo;

    public GetPersonsHandler(IPersonRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PersonListItemDto>> Handle(
        GetPersonsQuery request, CancellationToken cancellationToken)
    {
        var persons = await _repo.SearchAsync(
            request.Search, request.Court,
            Math.Clamp(request.Limit, 1, 100),
            request.ListView,
            cancellationToken);

        return persons.Select(p => new PersonListItemDto(
            p.Id,
            p.DisplayName,
            p.CourtName,
            p.RulingCount
        )).ToList();
    }
}
