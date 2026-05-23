namespace LegalAiAr.Core.Models;

/// <summary>
/// Paginated search result.
/// </summary>
/// <param name="Items">Page of search result items.</param>
/// <param name="TotalCount">Total number of matching results.</param>
public record PagedSearchResult(
    IReadOnlyList<SearchResultItem> Items,
    int TotalCount);
