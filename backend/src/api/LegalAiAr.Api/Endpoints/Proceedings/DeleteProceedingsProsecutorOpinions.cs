using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Commands;
using LegalAiAr.Application.Proceedings.Models;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class DeleteProceedingsProsecutorOpinions : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("/prosecutor-opinions", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteProsecutorOpinionsCommand(), cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization("AdminOnly")
        .WithName("DeleteProceedingsProsecutorOpinions")
        .WithTags("Proceedings")
        .Produces<DeleteProsecutorOpinionsResult>(StatusCodes.Status200OK);
}
