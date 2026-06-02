using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Commands;
using LegalAiAr.Application.Proceedings.Models;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class PostProceedingsLink : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/link", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new LinkProceedingsCommand(), cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization("AdminOnly")
        .WithName("PostProceedingsLink")
        .WithTags("Proceedings")
        .Produces<LinkProceedingsResult>(StatusCodes.Status200OK);
}
