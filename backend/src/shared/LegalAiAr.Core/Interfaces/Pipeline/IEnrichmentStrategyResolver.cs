namespace LegalAiAr.Core.Interfaces.Pipeline;

/// <summary>
/// Resolves the appropriate enrichment strategy for a given source ID.
/// </summary>
public interface IEnrichmentStrategyResolver
{
    /// <summary>
    /// Gets the enrichment strategy for the given source ID.
    /// </summary>
    /// <param name="sourceId">Source ID (CSJN=1, SAIJ=2, PJN=3, SCBA=4).</param>
    /// <returns>The enrichment strategy for the source.</returns>
    /// <exception cref="ArgumentException">Thrown when the source ID is not supported.</exception>
    IEnrichmentStrategy GetStrategy(int sourceId);
}
