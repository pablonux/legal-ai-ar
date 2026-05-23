using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Thesaurus.DTOs;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusChildren;

public record GetThesaurusChildrenQuery(int TermId) : IRequest<IReadOnlyList<ThesaurusTermDto>>;
