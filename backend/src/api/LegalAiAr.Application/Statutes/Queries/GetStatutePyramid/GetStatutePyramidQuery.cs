using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Statutes;

namespace LegalAiAr.Application.Statutes.Queries.GetStatutePyramid;

public record GetStatutePyramidQuery() : IRequest<IReadOnlyList<PyramidLevelDto>>;
