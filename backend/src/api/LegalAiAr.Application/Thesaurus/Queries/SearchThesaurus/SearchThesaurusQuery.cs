using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Thesaurus.DTOs;

namespace LegalAiAr.Application.Thesaurus.Queries.SearchThesaurus;

public record SearchThesaurusQuery(string Search, int Limit = 10) : IRequest<IReadOnlyList<ThesaurusTermDto>>;
