using LegalAiAr.Application.Graph.Models;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Graph.Queries;

public record GetNeighborhoodQuery(string EntityType, string EntityId) : IRequest<NeighborhoodResponse>;

public class GetNeighborhoodHandler : IRequestHandler<GetNeighborhoodQuery, NeighborhoodResponse>
{
    private readonly IGraphExplorerRepository _repo;

    public GetNeighborhoodHandler(IGraphExplorerRepository repo) => _repo = repo;

    public async Task<NeighborhoodResponse> Handle(GetNeighborhoodQuery request, CancellationToken cancellationToken)
    {
        var raw = await _repo.GetNeighborhoodAsync(request.EntityType, request.EntityId, cancellationToken);

        return new NeighborhoodResponse(
            new GraphEntityNode(raw.Center.Id, raw.Center.EntityType, raw.Center.Label, raw.Center.Subtitle, raw.Center.Properties),
            raw.Nodes.Select(n => new GraphEntityNode(n.Id, n.EntityType, n.Label, n.Subtitle, n.Properties)).ToList(),
            raw.Edges.Select(e => new GraphEntityEdge(e.Id, e.Source, e.Target, e.Type, e.Label)).ToList());
    }
}
