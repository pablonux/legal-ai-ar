using LegalAiAr.Application.Rulings.DTOs;

namespace LegalAiAr.Application.Rulings.Queries.SearchRulings;

/// <summary>
/// Paginated search result for rulings.
/// </summary>
public record SearchRulingsResult(
    int TotalCount,
    int Page,
    int PageSize,
    IReadOnlyList<RulingSearchResultDto> Results);
