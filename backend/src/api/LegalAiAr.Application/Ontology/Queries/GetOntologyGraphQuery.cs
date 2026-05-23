using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Ontology.Queries;

public record GetOntologyGraphQuery : IRequest<OntologyGraphResponse>;

public class GetOntologyGraphHandler : IRequestHandler<GetOntologyGraphQuery, OntologyGraphResponse>
{
    private readonly OntologyModelProvider _model;
    private readonly IOntologyStatsProvider _stats;

    public GetOntologyGraphHandler(OntologyModelProvider model, IOntologyStatsProvider stats)
    {
        _model = model;
        _stats = stats;
    }

    public async Task<OntologyGraphResponse> Handle(GetOntologyGraphQuery request, CancellationToken cancellationToken)
    {
        var (templateNodes, templateEdges) = _model.GetGraph();
        var counts = await _stats.GetEntityCountsAsync(cancellationToken);
        var relationCounts = await _stats.GetRelationCountsAsync(cancellationToken);

        var nodes = templateNodes.Select(n =>
            n with { InstanceCount = counts.GetValueOrDefault(n.Id) }
        ).ToList();

        var edges = templateEdges.Select(e =>
            e.Type == "relationship"
                ? e with { InstanceCount = relationCounts.GetValueOrDefault(e.Label) }
                : e
        ).ToList();

        return new OntologyGraphResponse(nodes, edges);
    }
}
