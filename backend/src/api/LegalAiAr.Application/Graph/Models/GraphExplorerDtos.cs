namespace LegalAiAr.Application.Graph.Models;

public record NeighborhoodResponse(
    GraphEntityNode Center,
    IReadOnlyList<GraphEntityNode> Nodes,
    IReadOnlyList<GraphEntityEdge> Edges);

public record GraphEntityNode(
    string Id,
    string EntityType,
    string Label,
    string? Subtitle,
    Dictionary<string, string>? Properties);

public record GraphEntityEdge(
    string Id,
    string Source,
    string Target,
    string Type,
    string? Label);

public record EntitySearchResult(
    string Id,
    string EntityType,
    string Label,
    string? Subtitle);

public record EntitySearchResponse(IReadOnlyList<EntitySearchResult> Results);
