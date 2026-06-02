using LegalAiAr.Application.Mapping.Rulings;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Rulings;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Rulings.Queries.GetSearchFacets;

/// <summary>
/// Handles retrieval of facet values for search filter dropdowns.
/// </summary>
public class GetSearchFacetsHandler : IRequestHandler<GetSearchFacetsQuery, SearchFacetsResponse>
{
    private readonly ISearchService _search;

    public GetSearchFacetsHandler(ISearchService search)
    {
        _search = search;
    }

    /// <inheritdoc />
    public async Task<SearchFacetsResponse> Handle(
        GetSearchFacetsQuery request,
        CancellationToken cancellationToken)
    {
        var facets = await _search.GetFacetsAsync(cancellationToken);
        return facets.ToResponse();
    }
}
