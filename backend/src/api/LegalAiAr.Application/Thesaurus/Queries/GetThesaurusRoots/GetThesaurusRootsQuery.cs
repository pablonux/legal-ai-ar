using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusRoots;

public record GetThesaurusRootsQuery : IRequest<IReadOnlyList<ThesaurusTermDto>>;
