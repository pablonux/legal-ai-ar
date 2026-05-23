namespace LegalAiAr.Application.Rulings.DTOs;

/// <summary>
/// Statute cited in a ruling.
/// </summary>
public record StatuteDto(
    string Number,
    string Name,
    string? Articles,
    string? Url,
    string? NormType = null,
    string? NormativeLevel = null,
    string? LegalBranch = null,
    string? IssuingBody = null,
    string? SanctionDate = null,
    string? EffectiveFrom = null,
    string? EffectiveTo = null);
