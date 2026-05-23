namespace LegalAiAr.Application.Proceedings.Models;

public record ProceedingResponse(
    int Id,
    string CaseNumber,
    string? DisplayName,
    string? JurisdictionArea,
    IReadOnlyList<ProceedingRulingDto> Rulings);

public record ProceedingRulingDto(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string CourtName,
    int? InstanceLevel,
    string? RulingDirection,
    bool IsCurrent);

public record ProsecutorOpinionDto(
    string ProsecutorName,
    string? Summary,
    string? RecommendedDirection,
    bool AgreedWithCourt);

public record LinkProceedingsResult(int ProceedingsCreated, int RulingsLinked);
