using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Statutes;
using LegalAiAr.Application.Statutes.Queries.GetStatuteById;

namespace LegalAiAr.Api.Endpoints.Statutes;

public sealed class GetStatuteById : IEndpoint
{
    public string GroupName => StatutesEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetStatuteByIdQuery(id), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetStatuteById")
        .WithTags("Statutes")
        .Produces<StatuteDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
