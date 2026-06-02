namespace LegalAiAr.Contracts.Requests.Rulings;

/// <summary>
/// Request body for POST /api/rulings/search.
/// </summary>
public record SearchRulingsRequest(
    string? Query = null,
    SearchFiltersRequest? Filters = null,
    int Page = 1,
    int PageSize = 10);

/// <summary>
/// Optional filters for semantic search.
/// </summary>
public record SearchFiltersRequest(
    string? JurisdictionArea = null,
    string? Instance = null,
    int? CourtId = null,
    string? Court = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    IReadOnlyList<string>? Keywords = null,
    string? SubjectArea = null,
    string? ResourceType = null,
    bool? IsUnconstitutional = null);
