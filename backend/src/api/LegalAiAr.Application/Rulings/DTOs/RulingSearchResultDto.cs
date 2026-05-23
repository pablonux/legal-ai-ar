namespace LegalAiAr.Application.Rulings.DTOs;

/// <summary>
/// Single ruling in a search result.
/// </summary>
public record RulingSearchResultDto(
    Guid Id,
    string CaseTitle,
    string? Summary,
    string? Holding,
    DateOnly RulingDate,
    string? JurisdictionArea,
    string? Instance,
    string? Court,
    IReadOnlyList<string> Keywords,
    string? RulingDirection,
    double RelevanceScore,
    string? HighlightedText,
    string? LegalBranch = null,
    string? PrecedentWeight = null,
    bool IsPlenario = false,
    bool IsLeadingCase = false);
