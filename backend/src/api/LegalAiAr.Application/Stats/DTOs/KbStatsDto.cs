namespace LegalAiAr.Application.Stats.DTOs;

public record KbStatsDto(
    int TotalRulings,
    int TotalCourts,
    int TotalPersons,
    int TotalKeywords,
    int TotalStatutes,
    int TotalCitations,
    DateOnly? EarliestRulingDate,
    DateOnly? LatestRulingDate,
    IReadOnlyList<SourceStatDto> BySource,
    IReadOnlyList<YearStatDto> ByYear,
    IReadOnlyList<CourtStatDto> TopCourts,
    IReadOnlyList<NameCountDto> ByJurisdiction,
    IReadOnlyList<NameCountDto> ByInstance,
    IReadOnlyList<NameCountDto> BySubjectArea,
    IReadOnlyList<NameCountDto> TopKeywords,
    QualityStatsDto Quality);

public record SourceStatDto(int SourceId, string Name, int Count);

public record YearStatDto(int Year, int Count);

public record CourtStatDto(int CourtId, string Name, string JurisdictionArea, int Count);

public record NameCountDto(string Name, int Count);

public record QualityStatsDto(
    int WithSummary,
    int WithHolding,
    int WithFullText,
    int WithKeywords,
    int WithPersons,
    int WithStatutes,
    int WithCitations);
