using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Search;

namespace LegalAiAr.Application.Search.Queries;

public record GlobalSearchQuery(string Query, int MaxPerEntity = 5) : IRequest<GlobalSearchResultDto>;
