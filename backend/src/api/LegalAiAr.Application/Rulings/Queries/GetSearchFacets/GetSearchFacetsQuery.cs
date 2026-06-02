using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Rulings;

namespace LegalAiAr.Application.Rulings.Queries.GetSearchFacets;

/// <summary>
/// Query to retrieve facet values for search filter dropdowns.
/// </summary>
public record GetSearchFacetsQuery : IRequest<SearchFacetsResponse>;
