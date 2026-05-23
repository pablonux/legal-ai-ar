namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Provides graph neighborhood and entity search queries for the Knowledge Graph Explorer.
/// </summary>
public interface IGraphExplorerRepository
{
    Task<GraphNeighborhood> GetNeighborhoodAsync(string entityType, string entityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GraphSearchHit>> SearchEntitiesAsync(string query, string? types, CancellationToken cancellationToken = default);
}

public record GraphNeighborhood(
    GraphNodeRaw Center,
    IReadOnlyList<GraphNodeRaw> Nodes,
    IReadOnlyList<GraphEdgeRaw> Edges);

public record GraphNodeRaw(
    string Id,
    string EntityType,
    string Label,
    string? Subtitle,
    Dictionary<string, string>? Properties);

public record GraphEdgeRaw(
    string Id,
    string Source,
    string Target,
    string Type,
    string? Label);

public record GraphSearchHit(
    string Id,
    string EntityType,
    string Label,
    string? Subtitle);
