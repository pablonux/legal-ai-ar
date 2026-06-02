using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Commands;
using LegalAiAr.Application.Proceedings.Models;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class PostProceedingsExtractParties : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/extract-parties", async (
            int batchSize,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var size = batchSize == 0 ? 100 : batchSize;
            var result = await mediator.Send(new ExtractPartiesCommand(size), cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization("AdminOnly")
        .WithName("PostProceedingsExtractParties")
        .WithTags("Proceedings")
        .Produces<ExtractPartiesResult>(StatusCodes.Status200OK);
}
