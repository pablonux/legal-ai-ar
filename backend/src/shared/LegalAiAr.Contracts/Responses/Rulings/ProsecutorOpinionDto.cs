namespace LegalAiAr.Contracts.Responses.Rulings;

public record ProsecutorOpinionDto(
    string ProsecutorName,
    string? Summary,
    string? RecommendedDirection,
    bool AgreedWithCourt);
