namespace LegalAiAr.Application.Rulings.DTOs;

/// <summary>
/// Thematic keyword in a ruling detail response.
/// ThesaurusTermId links to the SAIJ thesaurus descriptor when matched.
/// </summary>
public record KeywordDto(
    int Id,
    string Description,
    int? ThesaurusTermId = null);
