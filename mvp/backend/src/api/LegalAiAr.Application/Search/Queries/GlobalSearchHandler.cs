using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Search.DTOs;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Search.Queries;

public class GlobalSearchHandler : IRequestHandler<GlobalSearchQuery, GlobalSearchResultDto>
{
    private readonly IGlobalSearchService _searchService;

    public GlobalSearchHandler(IGlobalSearchService searchService) => _searchService = searchService;

    public async Task<GlobalSearchResultDto> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        var items = await _searchService.SearchAsync(request.Query, request.MaxPerEntity, cancellationToken);

        var dtos = items.Select(i => new GlobalSearchItemDto(
            i.EntityType, i.Id, i.Title, i.Subtitle, i.Route
        )).ToList();

        return new GlobalSearchResultDto(dtos, dtos.Count);
    }
}
