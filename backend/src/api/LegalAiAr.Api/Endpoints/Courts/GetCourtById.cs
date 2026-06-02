using LegalAiAr.Api.Interfaces;
using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Catalogs.Queries.GetCourtById;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Courts;

public sealed class GetCourtById : IEndpoint
{
    public string GroupName => CourtsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetCourtByIdQuery(id), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetCourtById")
        .WithTags("Courts")
        .Produces<CourtDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
