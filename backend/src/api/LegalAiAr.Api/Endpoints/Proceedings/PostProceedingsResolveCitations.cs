using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Commands;
using LegalAiAr.Application.Proceedings.Models;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class PostProceedingsResolveCitations : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/resolve-citations", async (
            int batchSize,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var size = batchSize == 0 ? 200 : batchSize;
            var result = await mediator.Send(new ResolveCitationsCommand(size), cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization("AdminOnly")
        .WithName("PostProceedingsResolveCitations")
        .WithTags("Proceedings")
        .Produces<ResolveCitationsResult>(StatusCodes.Status200OK);
}
