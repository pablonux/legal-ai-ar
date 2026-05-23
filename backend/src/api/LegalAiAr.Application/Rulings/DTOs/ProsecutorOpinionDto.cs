namespace LegalAiAr.Application.Rulings.DTOs;

public record ProsecutorOpinionDto(
    string ProsecutorName,
    string? Summary,
    string? RecommendedDirection,
    bool AgreedWithCourt);
