namespace LegalAiAr.Application.Search.DTOs;

public record GlobalSearchResultDto(
    IReadOnlyList<GlobalSearchItemDto> Items,
    int TotalCount);

public record GlobalSearchItemDto(
    string EntityType,
    string Id,
    string Title,
    string? Subtitle,
    string Route);
