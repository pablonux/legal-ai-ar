using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Catalogs.Queries.GetCourtById;

public record GetCourtByIdQuery(int Id) : IRequest<CourtDetailDto?>;
