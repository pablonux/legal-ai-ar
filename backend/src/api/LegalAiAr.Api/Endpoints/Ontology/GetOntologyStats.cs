using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;
using LegalAiAr.Application.Ontology.Queries;

namespace LegalAiAr.Api.Endpoints.Ontology;

public sealed class GetOntologyStats : IEndpoint
{
    public string GroupName => OntologyEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/stats", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetOntologyStatsQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetOntologyStats")
        .WithTags("Ontology")
        .Produces<OntologyStatsResponse>(StatusCodes.Status200OK);
}
