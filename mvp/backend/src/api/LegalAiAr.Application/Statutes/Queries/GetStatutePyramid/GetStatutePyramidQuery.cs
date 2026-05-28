using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Statutes.DTOs;

namespace LegalAiAr.Application.Statutes.Queries.GetStatutePyramid;

public record GetStatutePyramidQuery() : IRequest<IReadOnlyList<PyramidLevelDto>>;
