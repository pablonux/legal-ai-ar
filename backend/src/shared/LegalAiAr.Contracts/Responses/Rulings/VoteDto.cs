namespace LegalAiAr.Contracts.Responses.Rulings;

public record VoteDto(
    int Id,
    string VoteType,
    string? Pages,
    string? Summary,
    IReadOnlyList<string> Judges);
