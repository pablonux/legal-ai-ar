using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Proceedings.DTOs;

public record ProceedingListItemDto(
    int Id,
    string CaseNumber,
    string? DisplayName,
    string? JurisdictionArea,
    ProcessType? ProcessType,
    LegalBranch? LegalBranch,
    ProcessStatus? Status,
    string? CourtName,
    int RulingCount,
    DateOnly? FirstRulingDate,
    DateOnly? LastRulingDate);

public record ProceedingPageDto(
    IReadOnlyList<ProceedingListItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record ProceedingDetailDto(
    int Id,
    string CaseNumber,
    string? DisplayName,
    string? JurisdictionArea,
    ProcessType? ProcessType,
    string? ProcessSubtype,
    LegalBranch? LegalBranch,
    ProcessStatus? Status,
    string? CourtName,
    int? CourtId,
    int RulingCount,
    DateOnly? FirstRulingDate,
    DateOnly? LastRulingDate,
    IReadOnlyList<ProceedingRulingDto> Rulings,
    IReadOnlyList<ProceedingPartyDto> Parties,
    IReadOnlyList<LegalRepresentationDto> Representations);

public record ProceedingRulingDto(
    Guid Id,
    string CaseTitle,
    DateOnly RulingDate,
    string? CourtName,
    string? Instance);

public record ProceedingPartyDto(
    int PersonId,
    string PersonName,
    string Role);

public record LegalRepresentationDto(
    int LawyerId,
    string LawyerName,
    int PartyId,
    string PartyName);

public record ProceduralRemedyDto(
    int Id,
    string RemedyType,
    DateOnly? FilingDate,
    DateOnly? ResolutionDate,
    string? Outcome,
    Guid? ResolvingRulingId,
    string? ResolvingRulingTitle,
    Guid? AppealedRulingId,
    string? AppealedRulingTitle,
    string? CourtAQuoName,
    string? CourtAdQuemName);

public record AppealChainDto(
    int ProceedingId,
    string CaseNumber,
    string? DisplayName,
    IReadOnlyList<AppealChainNodeDto> Nodes);

public record AppealChainNodeDto(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? Instance,
    string CourtName,
    IReadOnlyList<ProceduralRemedyDto> RemediesFromHere);
