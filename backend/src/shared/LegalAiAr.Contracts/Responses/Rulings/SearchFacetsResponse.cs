namespace LegalAiAr.Contracts.Responses.Rulings;

public record FacetValueResponse(string Value, long Count);

public record SearchFacetsResponse(
    IReadOnlyList<FacetValueResponse> JurisdictionAreas,
    IReadOnlyList<FacetValueResponse> Instances,
    IReadOnlyList<FacetValueResponse> Courts,
    IReadOnlyList<FacetValueResponse> CourtTypes,
    IReadOnlyList<FacetValueResponse> Fueros,
    IReadOnlyList<FacetValueResponse> SubjectAreas,
    IReadOnlyList<FacetValueResponse> LegalBranches,
    IReadOnlyList<FacetValueResponse> PrecedentWeights,
    IReadOnlyList<FacetValueResponse> ResourceTypes)
{
    public static readonly SearchFacetsResponse Empty = new([], [], [], [], [], [], [], [], []);
}
