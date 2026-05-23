namespace LegalAiAr.Application.Ontology.Models;

public record OntologyClassDto(
    string Id,
    string Name,
    string Description,
    string Namespace,
    string? ParentId,
    string Category,
    string? KbEntity,
    string? KbRoute,
    IReadOnlyList<OntologyPropertyDto> Properties,
    IReadOnlyList<string> Children);

public record OntologyPropertyDto(
    string Name,
    string Type,
    string Description,
    string? TaxonomyId);

public record OntologyClassesResponse(IReadOnlyList<OntologyClassDto> Classes);

public record OntologyGraphResponse(
    IReadOnlyList<GraphNodeDto> Nodes,
    IReadOnlyList<GraphEdgeDto> Edges);

public record GraphNodeDto(
    string Id,
    string Label,
    string Category,
    int InstanceCount,
    string? KbRoute);

public record GraphEdgeDto(
    string Id,
    string Source,
    string Target,
    string Type,
    string Label,
    int InstanceCount = 0);

public record OntologyStatsResponse(IReadOnlyList<EntityStatsDto> Entities);

public record EntityStatsDto(
    string ClassId,
    string KbEntity,
    int TotalCount,
    IReadOnlyList<TaxonomyBreakdownDto> Breakdowns);

public record TaxonomyBreakdownDto(
    string TaxonomyId,
    string TaxonomyName,
    IReadOnlyList<TaxonomyValueCountDto> Values);

public record TaxonomyValueCountDto(
    string Code,
    string Label,
    int Count);

public record TaxonomyResponse(
    string Id,
    string Name,
    string Description,
    IReadOnlyList<TaxonomyValueDto> Values);

public record TaxonomyValueDto(
    string Code,
    string Label,
    string? Group,
    int Count,
    string? Description);
