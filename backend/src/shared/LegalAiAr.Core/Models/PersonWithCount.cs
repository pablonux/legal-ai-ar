namespace LegalAiAr.Core.Models;

public sealed record PersonWithCount(
    int PersonId,
    string FirstName,
    string LastName,
    int RulingCount);
