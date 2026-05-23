using LegalAiAr.Application.Rulings.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Rulings.Queries.GetRelatedRulings;

/// <summary>
/// Query for rulings semantically similar to the given ruling.
/// </summary>
public record GetRelatedRulingsQuery(Guid Id, int Limit = 10)
    : IRequest<IReadOnlyList<RelatedRulingDto>>;
