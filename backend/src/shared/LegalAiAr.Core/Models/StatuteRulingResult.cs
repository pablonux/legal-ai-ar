namespace LegalAiAr.Core.Models;

public sealed record StatuteRulingResult(
    Guid RulingId,
    string CaseTitle,
    DateOnly RulingDate,
    string? CourtName,
    string? Summary,
    string StatuteName,
    string? StatuteNumber,
    string? Articles);
