using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;

namespace LegalAiAr.Application.Thesaurus.Queries.SearchThesaurus;

public record SearchThesaurusQuery(string Search, int Limit = 10) : IRequest<IReadOnlyList<ThesaurusTermDto>>;
