using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Search.DTOs;

namespace LegalAiAr.Application.Search.Queries;

public record GlobalSearchQuery(string Query, int MaxPerEntity = 5) : IRequest<GlobalSearchResultDto>;
