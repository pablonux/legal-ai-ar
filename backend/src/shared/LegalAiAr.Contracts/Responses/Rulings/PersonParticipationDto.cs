namespace LegalAiAr.Contracts.Responses.Rulings;

/// <summary>
/// Person who participated in a ruling, with their role.
/// </summary>
public record PersonParticipationDto(
    int PersonId,
    string DisplayName,
    string RulingRole);
