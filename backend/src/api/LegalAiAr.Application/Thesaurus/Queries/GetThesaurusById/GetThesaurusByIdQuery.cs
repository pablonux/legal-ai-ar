using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Thesaurus.DTOs;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusById;

public record GetThesaurusByIdQuery(int Id) : IRequest<ThesaurusTermDetailDto?>;
