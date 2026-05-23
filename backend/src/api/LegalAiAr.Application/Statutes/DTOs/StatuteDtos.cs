using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Statutes.DTOs;

public record StatuteListItemDto(
    int Id,
    string Number,
    string Name,
    NormType? NormType,
    NormativeLevel? NormativeLevel,
    LegalBranch? LegalBranch,
    string? IssuingBody,
    DateOnly? SanctionDate,
    string? Status,
    bool IsVigente,
    int RulingCount);

public record StatuteDetailDto(
    int Id,
    string Number,
    string Name,
    string? Url,
    NormType? NormType,
    NormativeLevel? NormativeLevel,
    LegalBranch? LegalBranch,
    string? IssuingBody,
    string? IssuingBodyName,
    DateOnly? SanctionDate,
    DateOnly? PublicationDate,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    string? OfficialUrl,
    string? SaijId,
    string? Status,
    bool HasFullText,
    bool IsVigente,
    int RulingCount,
    IReadOnlyList<StatuteRulingDto> RecentRulings,
    IReadOnlyList<NormRelationDto> Relations);

public record StatuteRulingDto(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? CourtName,
    string? Articles);

public record NormRelationDto(
    int RelatedStatuteId,
    string RelatedStatuteNumber,
    string RelatedStatuteName,
    string RelationType,
    bool IsOutbound);

public record PyramidLevelDto(
    NormativeLevel Level,
    string Label,
    int Count,
    int VigenteCount);
