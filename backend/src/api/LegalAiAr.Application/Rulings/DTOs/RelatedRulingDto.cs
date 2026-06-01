namespace LegalAiAr.Application.Rulings.DTOs;

/// <summary>
/// Related ruling by semantic similarity.
/// </summary>
public record RelatedRulingDto(
    Guid Id,
    string CaseTitle,
    DateOnly RulingDate,
    string? JurisdictionArea,
    string? Instance,
    double SimilarityScore);
