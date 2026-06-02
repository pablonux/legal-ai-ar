namespace LegalAiAr.Contracts.Responses.Rulings;

/// <summary>
/// Paginated search result for rulings.
/// </summary>
public record SearchRulingsResult(
    int TotalCount,
    int Page,
    int PageSize,
    IReadOnlyList<RulingSearchResultDto> Results);
