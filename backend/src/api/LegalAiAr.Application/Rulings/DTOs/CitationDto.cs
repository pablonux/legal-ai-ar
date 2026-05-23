namespace LegalAiAr.Application.Rulings.DTOs;

/// <summary>
/// Citation from one ruling to another.
/// </summary>
public record CitationDto(
    string ExternalAlias,
    string CitationType,
    Guid? TargetRulingId,
    string? TargetCaseTitle);
