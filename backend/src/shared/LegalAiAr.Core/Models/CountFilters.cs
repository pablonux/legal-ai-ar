namespace LegalAiAr.Core.Models;

public sealed record CountFilters(
    string? JurisdictionArea = null,
    string? Instance = null,
    string? CourtName = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    bool? IsUnconstitutional = null,
    IReadOnlyList<string>? Keywords = null);
