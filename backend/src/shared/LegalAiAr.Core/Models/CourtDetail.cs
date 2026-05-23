using LegalAiAr.Core.Entities;

namespace LegalAiAr.Core.Models;

public sealed record CourtDetail(
    Court Court,
    int RulingCount,
    IReadOnlyList<PersonWithCount> TopPersons);
