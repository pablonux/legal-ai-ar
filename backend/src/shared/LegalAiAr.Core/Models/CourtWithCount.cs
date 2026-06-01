namespace LegalAiAr.Core.Models;

public sealed record CourtWithCount(
    int Id,
    string Name,
    string JurisdictionArea,
    string Territory,
    string Instance,
    int RulingCount);
