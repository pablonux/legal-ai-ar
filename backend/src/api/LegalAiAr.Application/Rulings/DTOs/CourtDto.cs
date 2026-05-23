namespace LegalAiAr.Application.Rulings.DTOs;

/// <summary>
/// Court/tribunal in a ruling detail response.
/// </summary>
public record CourtDto(
    int Id,
    string Name,
    string JurisdictionArea,
    string Territory,
    string Instance,
    string? CourtCategory = null,
    string? Fuero = null,
    int? InstanceLevel = null,
    string? GovernmentLevel = null);
