namespace LegalAiAr.Core.Models;

public sealed record PersonListItem(
    int Id,
    string DisplayName,
    string? CourtName,
    int RulingCount);
