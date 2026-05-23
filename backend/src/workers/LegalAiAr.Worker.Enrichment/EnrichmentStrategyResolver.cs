using LegalAiAr.Core.Interfaces.Pipeline;

namespace LegalAiAr.Worker.Enrichment;

/// <summary>
/// Resolves enrichment strategy by source ID. Phase 1: only CSJN (sourceId=1) supported.
/// </summary>
public class EnrichmentStrategyResolver : IEnrichmentStrategyResolver
{
    private readonly IEnrichmentStrategy _csjnStrategy;

    public EnrichmentStrategyResolver(IEnrichmentStrategy csjnStrategy)
    {
        _csjnStrategy = csjnStrategy;
    }

    /// <inheritdoc />
    public IEnrichmentStrategy GetStrategy(int sourceId)
    {
        return sourceId switch
        {
            1 => _csjnStrategy,
            _ => throw new ArgumentException($"Enrichment strategy for source {sourceId} is not implemented in Phase 1. Only CSJN (1) is supported.", nameof(sourceId))
        };
    }
}
