using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Rulings.Queries.GetSearchFacets;

/// <summary>
/// Handles retrieval of facet values for search filter dropdowns.
/// </summary>
public class GetSearchFacetsHandler : IRequestHandler<GetSearchFacetsQuery, SearchFacets>
{
    private readonly ISearchService _search;

    public GetSearchFacetsHandler(ISearchService search)
    {
        _search = search;
    }

    /// <inheritdoc />
    public async Task<SearchFacets> Handle(
        GetSearchFacetsQuery request,
        CancellationToken cancellationToken)
    {
        return await _search.GetFacetsAsync(cancellationToken);
    }
}
