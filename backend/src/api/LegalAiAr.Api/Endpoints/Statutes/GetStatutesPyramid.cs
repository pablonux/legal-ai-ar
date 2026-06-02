using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Statutes;
using LegalAiAr.Application.Statutes.Queries.GetStatutePyramid;

namespace LegalAiAr.Api.Endpoints.Statutes;

public sealed class GetStatutesPyramid : IEndpoint
{
    public string GroupName => StatutesEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/pyramid", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetStatutePyramidQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetStatutesPyramid")
        .WithTags("Statutes")
        .Produces<IReadOnlyList<PyramidLevelDto>>(StatusCodes.Status200OK);
}
