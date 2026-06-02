using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Catalogs.Queries.GetPersons;

public record GetPersonsQuery(
    string? Search,
    string? Court,
    int Limit = 50,
    PersonListView ListView = PersonListView.All) : IRequest<IReadOnlyList<PersonListItemDto>>;
