using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Ontology.Queries;

public record GetOntologyStatsQuery : IRequest<OntologyStatsResponse>;

public class GetOntologyStatsHandler : IRequestHandler<GetOntologyStatsQuery, OntologyStatsResponse>
{
    private readonly IOntologyStatsProvider _stats;

    public GetOntologyStatsHandler(IOntologyStatsProvider stats) => _stats = stats;

    public async Task<OntologyStatsResponse> Handle(GetOntologyStatsQuery request, CancellationToken cancellationToken)
    {
        var raw = await _stats.GetEntityStatsAsync(cancellationToken);

        var entities = raw.Select(e => new EntityStatsDto(
            e.ClassId, e.KbEntity, e.TotalCount,
            e.Breakdowns.Select(b => new TaxonomyBreakdownDto(
                b.TaxonomyId, b.TaxonomyName,
                b.Values.Select(v => new TaxonomyValueCountDto(v.Code, v.Label, v.Count)).ToList()
            )).ToList()
        )).ToList();

        return new OntologyStatsResponse(entities);
    }
}
