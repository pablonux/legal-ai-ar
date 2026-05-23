using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Interfaces.Pipeline.Strategies;

/// <summary>
/// Resolves the appropriate strategy implementation for a given (EntityType, SourceId) combination.
/// Each worker registers concrete strategies and uses this resolver to dispatch processing.
/// </summary>
public interface IStrategyResolver<out TStrategy>
{
    TStrategy Resolve(EntityType entityType, int sourceId);
}
