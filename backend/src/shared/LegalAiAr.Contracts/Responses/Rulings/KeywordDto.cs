namespace LegalAiAr.Contracts.Responses.Rulings;

/// <summary>
/// Thematic keyword in a ruling detail response.
/// </summary>
public record KeywordDto(
    int Id,
    string Description,
    int? ThesaurusTermId = null);
