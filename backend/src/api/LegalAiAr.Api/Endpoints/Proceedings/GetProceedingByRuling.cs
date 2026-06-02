using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Models;
using LegalAiAr.Application.Proceedings.Queries;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class GetProceedingByRuling : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/by-ruling/{rulingId:guid}", async (
            Guid rulingId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetProceedingByRulingQuery(rulingId), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetProceedingByRuling")
        .WithTags("Proceedings")
        .Produces<ProceedingResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
