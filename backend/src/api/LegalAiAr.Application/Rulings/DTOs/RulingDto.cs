namespace LegalAiAr.Application.Rulings.DTOs;

/// <summary>
/// Full ruling detail response.
/// </summary>
public record RulingDto(
    Guid Id,
    int SourceId,
    string ExternalId,
    string CaseTitle,
    string? CaseNumber,
    DateOnly RulingDate,
    CourtDto Court,
    string? JurisdictionArea,
    string? Instance,
    string? Jurisdiction,
    string? ResourceType,
    string? RulingDirection,
    string? SubjectArea,
    string? LegalBranch,
    string? PrecedentWeight,
    bool IsPlenario,
    bool IsLeadingCase,
    bool IsUnconstitutional,
    string? Summary,
    string? Holding,
    string? FullText,
    string? BlobPath,
    string? RatioDecidendi,
    string? DoctrinaLegal,
    DateTime IndexedAt,
    string Status,
    IReadOnlyList<PersonParticipationDto> Persons,
    IReadOnlyList<KeywordDto> Keywords,
    IReadOnlyList<StatuteDto> Statutes,
    IReadOnlyList<CitationDto> Citations,
    IReadOnlyList<VoteDto> Votes,
    IReadOnlyList<LegalDoctrineDto> Doctrines,
    ProsecutorOpinionDto? ProsecutorOpinion = null);
