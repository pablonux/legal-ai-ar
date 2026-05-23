using LegalAiAr.Core.Models;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Rulings.Queries.GetSearchFacets;

/// <summary>
/// Query to retrieve facet values for search filter dropdowns.
/// </summary>
public record GetSearchFacetsQuery : IRequest<SearchFacets>;
