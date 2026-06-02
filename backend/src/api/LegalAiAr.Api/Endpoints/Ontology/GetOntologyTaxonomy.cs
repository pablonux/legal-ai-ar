using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;
using LegalAiAr.Application.Ontology.Queries;

namespace LegalAiAr.Api.Endpoints.Ontology;

public sealed class GetOntologyTaxonomy : IEndpoint
{
    public string GroupName => OntologyEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/taxonomies/{taxonomyId}", async (
            string taxonomyId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetTaxonomyValuesQuery(taxonomyId), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetOntologyTaxonomy")
        .WithTags("Ontology")
        .Produces<TaxonomyResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
