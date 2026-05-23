using LegalAiAr.Application.Catalogs.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Catalogs.Queries.GetCourtById;

public record GetCourtByIdQuery(int Id) : IRequest<CourtDetailDto?>;
