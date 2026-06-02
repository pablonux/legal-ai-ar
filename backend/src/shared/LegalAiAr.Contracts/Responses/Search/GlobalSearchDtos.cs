namespace LegalAiAr.Contracts.Responses.Search;

public record GlobalSearchResultDto(
    IReadOnlyList<GlobalSearchItemDto> Items,
    int TotalCount);

public record GlobalSearchItemDto(
    string EntityType,
    string Id,
    string Title,
    string? Subtitle,
    string Route);
