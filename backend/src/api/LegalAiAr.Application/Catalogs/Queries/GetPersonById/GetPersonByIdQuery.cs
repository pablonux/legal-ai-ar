using LegalAiAr.Application.Catalogs.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Catalogs.Queries.GetPersonById;

public record GetPersonByIdQuery(int Id) : IRequest<PersonDetailDto?>;
