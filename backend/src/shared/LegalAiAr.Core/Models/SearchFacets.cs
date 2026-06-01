namespace LegalAiAr.Core.Models;

public record FacetValue(string Value, long Count);

public record SearchFacets(
    IReadOnlyList<FacetValue> JurisdictionAreas,
    IReadOnlyList<FacetValue> Instances,
    IReadOnlyList<FacetValue> Courts,
    IReadOnlyList<FacetValue> CourtTypes,
    IReadOnlyList<FacetValue> Fueros,
    IReadOnlyList<FacetValue> SubjectAreas,
    IReadOnlyList<FacetValue> LegalBranches,
    IReadOnlyList<FacetValue> PrecedentWeights,
    IReadOnlyList<FacetValue> ResourceTypes)
{
    public static readonly SearchFacets Empty = new([], [], [], [], [], [], [], [], []);
}
