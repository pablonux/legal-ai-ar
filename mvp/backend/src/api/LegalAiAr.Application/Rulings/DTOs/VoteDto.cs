namespace LegalAiAr.Application.Rulings.DTOs;

public record VoteDto(
    int Id,
    string VoteType,
    string? Pages,
    string? Summary,
    IReadOnlyList<string> Judges);
