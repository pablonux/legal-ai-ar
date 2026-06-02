using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Rulings.Queries.GetRelatedRulings;

namespace LegalAiAr.Api.Endpoints.Rulings;

public sealed class GetRulingRelated : IEndpoint
{
    public string GroupName => RulingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:guid}/related", async (
            Guid id,
            int? limit,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRelatedRulingsQuery(id, limit ?? 10);
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetRulingRelated")
        .WithTags("Rulings")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
