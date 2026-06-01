using LegalAiAr.Application.Graph.Models;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Graph.Queries;

public record SearchEntitiesQuery(string Query, string? Types) : IRequest<EntitySearchResponse>;

public class SearchEntitiesHandler : IRequestHandler<SearchEntitiesQuery, EntitySearchResponse>
{
    private readonly IGraphExplorerRepository _repo;

    public SearchEntitiesHandler(IGraphExplorerRepository repo) => _repo = repo;

    public async Task<EntitySearchResponse> Handle(SearchEntitiesQuery request, CancellationToken cancellationToken)
    {
        var hits = await _repo.SearchEntitiesAsync(request.Query, request.Types, cancellationToken);

        return new EntitySearchResponse(
            hits.Select(h => new EntitySearchResult(h.Id, h.EntityType, h.Label, h.Subtitle)).ToList());
    }
}
