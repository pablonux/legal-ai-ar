using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Ontology.Queries;

public record GetTaxonomyValuesQuery(string TaxonomyId) : IRequest<TaxonomyResponse?>;

public class GetTaxonomyValuesHandler : IRequestHandler<GetTaxonomyValuesQuery, TaxonomyResponse?>
{
    private readonly OntologyModelProvider _model;
    private readonly IOntologyStatsProvider _stats;

    public GetTaxonomyValuesHandler(OntologyModelProvider model, IOntologyStatsProvider stats)
    {
        _model = model;
        _stats = stats;
    }

    public async Task<TaxonomyResponse?> Handle(GetTaxonomyValuesQuery request, CancellationToken cancellationToken)
    {
        var taxonomies = _model.GetTaxonomies();
        if (!taxonomies.TryGetValue(request.TaxonomyId, out var taxonomy))
            return null;

        var counts = await _stats.GetTaxonomyCountsAsync(request.TaxonomyId, cancellationToken);

        var values = taxonomy.Values.Select(v => new TaxonomyValueDto(
            v.Code,
            v.Label,
            v.Group,
            counts.GetValueOrDefault(v.Code),
            v.Description
        )).ToList();

        return new TaxonomyResponse(taxonomy.Id, taxonomy.Name, taxonomy.Description, values);
    }
}
