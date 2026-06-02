using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Graph.Models;
using LegalAiAr.Application.Graph.Queries;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Graph;

public sealed class GetGraphNeighborhood : IEndpoint
{
    public string GroupName => GraphEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/neighborhood/{entityType}/{entityId}", async (
            string entityType,
            string entityId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await mediator.Send(new GetNeighborhoodQuery(entityType, entityId), cancellationToken);
                return Results.Ok(result);
            }
            catch (ArgumentException)
            {
                return Results.NotFound();
            }
        })
        .WithName("GetGraphNeighborhood")
        .WithTags("Graph")
        .Produces<NeighborhoodResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}

internal static class GraphEndpointGroup
{
    public const string Name = "graph";
}
