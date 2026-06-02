using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.DTOs;
using LegalAiAr.Application.Proceedings.Queries;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class GetProceedingById : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetProceedingByIdQuery(id), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetProceedingById")
        .WithTags("Proceedings")
        .Produces<ProceedingDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
