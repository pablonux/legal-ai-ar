using System.Security.Claims;
using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Rulings.Queries.GetRulingById;

namespace LegalAiAr.Api.Endpoints.Rulings;

public sealed class GetRulingById : IEndpoint
{
    public string GroupName => RulingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var allowReprocessing = user.IsInRole("admin");
            var query = new GetRulingByIdQuery(id, allowReprocessing);
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetRulingById")
        .WithTags("Rulings")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
