namespace LegalAiAr.Contracts.Responses.Catalogs;

public record CourtListItemDto(
    int Id,
    string Name,
    string JurisdictionArea,
    string Territory,
    string Instance,
    int RulingCount);

public record CourtDetailDto(
    int Id,
    string Name,
    string JurisdictionArea,
    string Territory,
    string Instance,
    int RulingCount,
    string? CourtCategory,
    string? Fuero,
    int? InstanceLevel,
    string? GovernmentLevel,
    CourtHierarchyNodeDto? ParentCourt,
    IReadOnlyList<CourtHierarchyNodeDto> ChildCourts,
    IReadOnlyList<JudicialOfficeDto> JudicialOffices,
    IReadOnlyList<PersonListItemDto> TopPersons);

public record CourtHierarchyNodeDto(
    int Id,
    string Name,
    string? Instance,
    int? InstanceLevel);

public record JudicialOfficeDto(
    int PersonId,
    string PersonName,
    string Position,
    string? StartDate,
    string? EndDate,
    bool IsCurrent);

public record PersonListItemDto(
    int Id,
    string DisplayName,
    string? CourtName,
    int RulingCount);

public record PersonDetailDto(
    int Id,
    string DisplayName,
    string? CourtName,
    int RulingCount,
    string PersonType,
    string? LegalEntityType,
    IReadOnlyList<PersonRecentRulingDto> RecentRulings,
    IReadOnlyList<PersonOfficeDto> JudicialOffices,
    IReadOnlyList<PersonProceedingDto> Proceedings);

public record PersonOfficeDto(
    int CourtId,
    string CourtName,
    string Position,
    string? StartDate,
    string? EndDate,
    bool IsCurrent);

public record PersonProceedingDto(
    int ProceedingId,
    string CaseNumber,
    string? DisplayName,
    string Role);

public record PersonRecentRulingDto(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? Instance,
    string RulingRole);
