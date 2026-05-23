namespace LegalAiAr.Core.Models;

/// <summary>
/// Raw KB statistics aggregated from SQL.
/// </summary>
public sealed record KbStatsRaw(
    int TotalRulings,
    int TotalCourts,
    int TotalPersons,
    int TotalKeywords,
    int TotalStatutes,
    int TotalCitations,
    DateOnly? EarliestRulingDate,
    DateOnly? LatestRulingDate,
    IReadOnlyList<SourceCount> BySource,
    IReadOnlyList<YearCount> ByYear,
    IReadOnlyList<CourtCount> TopCourts,
    IReadOnlyList<NameCount> ByJurisdiction,
    IReadOnlyList<NameCount> ByInstance,
    IReadOnlyList<NameCount> BySubjectArea,
    IReadOnlyList<NameCount> TopKeywords,
    QualityCounts Quality);

public sealed record SourceCount(int SourceId, string Name, int Count);
public sealed record YearCount(int Year, int Count);
public sealed record CourtCount(int CourtId, string Name, string JurisdictionArea, int Count);
public sealed record NameCount(string Name, int Count);
public sealed record QualityCounts(
    int WithSummary,
    int WithHolding,
    int WithFullText,
    int WithKeywords,
    int WithPersons,
    int WithStatutes,
    int WithCitations);
