using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;
using LegalAiAr.Application.Ontology.Queries;

namespace LegalAiAr.Api.Endpoints.Ontology;

public sealed class GetOntologyClasses : IEndpoint
{
    public string GroupName => OntologyEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/classes", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetOntologyClassesQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetOntologyClasses")
        .WithTags("Ontology")
        .Produces<OntologyClassesResponse>(StatusCodes.Status200OK);
}

internal static class OntologyEndpointGroup
{
    public const string Name = "ontology";
}
