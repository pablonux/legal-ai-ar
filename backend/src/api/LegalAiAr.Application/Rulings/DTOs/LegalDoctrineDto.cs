namespace LegalAiAr.Application.Rulings.DTOs;

public record LegalDoctrineDto(
    int Id,
    string Statement,
    string? Topic,
    bool IsOverruled,
    Guid? OverruledByRulingId,
    string? OverruledByRulingTitle);
