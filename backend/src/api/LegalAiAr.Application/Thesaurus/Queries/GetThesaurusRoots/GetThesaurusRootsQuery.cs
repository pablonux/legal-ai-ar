using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Thesaurus.DTOs;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusRoots;

public record GetThesaurusRootsQuery : IRequest<IReadOnlyList<ThesaurusTermDto>>;
