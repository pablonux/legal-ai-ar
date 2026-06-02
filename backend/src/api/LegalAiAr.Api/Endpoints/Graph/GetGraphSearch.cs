using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Graph.Models;
using LegalAiAr.Application.Graph.Queries;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Graph;

public sealed class GetGraphSearch : IEndpoint
{
    public string GroupName => GraphEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/search", async (
            string q,
            string? types,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Results.Ok(new EntitySearchResponse([]));

            var result = await mediator.Send(new SearchEntitiesQuery(q, types), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetGraphSearch")
        .WithTags("Graph")
        .Produces<EntitySearchResponse>(StatusCodes.Status200OK);
}
