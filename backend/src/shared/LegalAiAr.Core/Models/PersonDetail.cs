using LegalAiAr.Core.Entities;

namespace LegalAiAr.Core.Models;

public sealed record PersonDetail(
    Person Person,
    string? CourtName,
    int RulingCount,
    IReadOnlyList<PersonRecentRuling> RecentRulings);

public sealed record PersonRecentRuling(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? Instance,
    string RulingRole);
