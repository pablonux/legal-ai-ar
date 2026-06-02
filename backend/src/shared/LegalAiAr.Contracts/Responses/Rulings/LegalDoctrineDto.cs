namespace LegalAiAr.Contracts.Responses.Rulings;

public record LegalDoctrineDto(
    int Id,
    string Statement,
    string? Topic,
    bool IsOverruled,
    Guid? OverruledByRulingId,
    string? OverruledByRulingTitle);
