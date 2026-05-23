namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Provides KB instance counts for ontology classes and taxonomy values.
/// Implemented in Infrastructure against AppDbContext.
/// </summary>
public interface IOntologyStatsProvider
{
    Task<IReadOnlyDictionary<string, int>> GetEntityCountsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OntologyEntityStats>> GetEntityStatsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, int>> GetTaxonomyCountsAsync(string taxonomyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns instance counts for ontology relationship edges.
    /// Key = edge label (e.g. "citaFallo"), Value = count of instances.
    /// </summary>
    Task<IReadOnlyDictionary<string, int>> GetRelationCountsAsync(CancellationToken cancellationToken = default);
}

public record OntologyEntityStats(
    string ClassId,
    string KbEntity,
    int TotalCount,
    IReadOnlyList<OntologyTaxonomyBreakdown> Breakdowns);

public record OntologyTaxonomyBreakdown(
    string TaxonomyId,
    string TaxonomyName,
    IReadOnlyList<OntologyTaxonomyValueCount> Values);

public record OntologyTaxonomyValueCount(string Code, string Label, int Count);
