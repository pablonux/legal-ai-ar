using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusById;

public record GetThesaurusByIdQuery(int Id) : IRequest<ThesaurusTermDetailDto?>;
