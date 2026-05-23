namespace LegalAiAr.Application.Thesaurus.DTOs;

public record ThesaurusTermDto(
    int Id,
    string Label,
    string? Branch,
    int Depth);

public record ThesaurusTermDetailDto(
    int Id,
    string Label,
    string? Branch,
    int Depth,
    IReadOnlyList<ThesaurusRelationDto> Synonyms,
    IReadOnlyList<ThesaurusRelationDto> BroaderTerms,
    IReadOnlyList<ThesaurusRelationDto> NarrowerTerms,
    IReadOnlyList<ThesaurusRelationDto> RelatedTerms);

public record ThesaurusRelationDto(
    int Id,
    string Label);
