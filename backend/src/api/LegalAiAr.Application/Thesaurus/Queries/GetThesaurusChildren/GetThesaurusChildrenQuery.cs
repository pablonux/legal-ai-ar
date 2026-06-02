using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusChildren;

public record GetThesaurusChildrenQuery(int TermId) : IRequest<IReadOnlyList<ThesaurusTermDto>>;
