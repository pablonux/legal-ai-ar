using System.Collections.Frozen;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;

namespace LegalAiAr.Infrastructure.Pipeline;

/// <summary>
/// Generic strategy resolver. Workers register concrete strategies via
/// <see cref="StrategyResolverBuilder{TStrategy}"/>, which builds a frozen dictionary
/// keyed by (EntityType, SourceId).
/// </summary>
public sealed class StrategyResolver<TStrategy> : IStrategyResolver<TStrategy>
{
    private readonly FrozenDictionary<(EntityType, int), TStrategy> _strategies;

    public StrategyResolver(Dictionary<(EntityType, int), TStrategy> strategies)
    {
        _strategies = strategies.ToFrozenDictionary();
    }

    public TStrategy Resolve(EntityType entityType, int sourceId)
    {
        if (_strategies.TryGetValue((entityType, sourceId), out var strategy))
            return strategy;

        throw new InvalidOperationException(
            $"No {typeof(TStrategy).Name} registered for ({entityType}, SourceId={sourceId}).");
    }
}

/// <summary>
/// Fluent builder for registering strategy implementations.
/// Used in worker DI setup: <c>services.AddStrategyResolver&lt;IParseStrategy&gt;(b => b.Register(...));</c>
/// </summary>
public sealed class StrategyResolverBuilder<TStrategy>
{
    private readonly Dictionary<(EntityType, int), TStrategy> _strategies = new();

    public StrategyResolverBuilder<TStrategy> Register(EntityType entityType, int sourceId, TStrategy strategy)
    {
        _strategies[(entityType, sourceId)] = strategy;
        return this;
    }

    public StrategyResolver<TStrategy> Build() => new(_strategies);
}
