using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;
using LegalAiAr.Application.Ontology.Queries;

namespace LegalAiAr.Api.Endpoints.Ontology;

public sealed class GetOntologyGraph : IEndpoint
{
    public string GroupName => OntologyEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/graph", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetOntologyGraphQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetOntologyGraph")
        .WithTags("Ontology")
        .Produces<OntologyGraphResponse>(StatusCodes.Status200OK);
}
