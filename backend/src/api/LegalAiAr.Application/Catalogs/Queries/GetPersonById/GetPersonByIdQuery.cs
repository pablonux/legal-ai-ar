using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Catalogs.Queries.GetPersonById;

public record GetPersonByIdQuery(int Id) : IRequest<PersonDetailDto?>;
