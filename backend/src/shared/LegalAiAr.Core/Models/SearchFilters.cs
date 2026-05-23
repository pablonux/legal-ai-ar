namespace LegalAiAr.Core.Models;

/// <summary>
/// Filters for semantic search of rulings.
/// </summary>
/// <param name="JurisdictionArea">Filter by jurisdiction area.</param>
/// <param name="Instance">Filter by instance (e.g. CSJN, Cámara).</param>
/// <param name="CourtId">Filter by court ID. Resolved to CourtName by handler before search.</param>
/// <param name="CourtName">Filter by court name. Used by search service when index has court (name) not courtId.</param>
/// <param name="DateFrom">Filter rulings from this date.</param>
/// <param name="DateTo">Filter rulings until this date.</param>
/// <param name="Keywords">Filter by keyword descriptions.</param>
/// <param name="SubjectArea">Filter by subject area (e.g. Tributario, Penal).</param>
/// <param name="ResourceType">Filter by resource type (e.g. RECURSO EXTRAORDINARIO FEDERAL).</param>
/// <param name="IsUnconstitutional">Filter by unconstitutionality declaration.</param>
public record SearchFilters(
    string? JurisdictionArea = null,
    string? Instance = null,
    int? CourtId = null,
    string? CourtName = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    IReadOnlyList<string>? Keywords = null,
    string? SubjectArea = null,
    string? ResourceType = null,
    bool? IsUnconstitutional = null,
    string? CourtType = null,
    string? Fuero = null,
    string? LegalBranch = null,
    string? PrecedentWeight = null,
    string? ActionType = null,
    string? OfficialReference = null,
    bool? HasDictamen = null);
