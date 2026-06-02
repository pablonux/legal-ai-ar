using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.DTOs;
using LegalAiAr.Application.Proceedings.Queries;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class GetProceedingAppealChain : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}/appeal-chain", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetAppealChainQuery(id), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetProceedingAppealChain")
        .WithTags("Proceedings")
        .Produces<AppealChainDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
