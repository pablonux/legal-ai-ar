using LegalAiAr.Contracts.Responses.Rulings;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Mapping.Rulings;

public static class SearchFacetsMapper
{
    public static SearchFacetsResponse ToResponse(this SearchFacets facets) =>
        new(
            Map(facets.JurisdictionAreas),
            Map(facets.Instances),
            Map(facets.Courts),
            Map(facets.CourtTypes),
            Map(facets.Fueros),
            Map(facets.SubjectAreas),
            Map(facets.LegalBranches),
            Map(facets.PrecedentWeights),
            Map(facets.ResourceTypes));

    private static IReadOnlyList<FacetValueResponse> Map(IReadOnlyList<FacetValue> values) =>
        values.Select(v => new FacetValueResponse(v.Value, v.Count)).ToList();
}
