using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Commands;
using LegalAiAr.Application.Proceedings.Models;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class PostProceedingsExtractDoctrine : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/extract-doctrine", async (
            int batchSize,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var size = batchSize == 0 ? 50 : batchSize;
            var result = await mediator.Send(new ExtractDoctrineCommand(size), cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization("AdminOnly")
        .WithName("PostProceedingsExtractDoctrine")
        .WithTags("Proceedings")
        .Produces<ExtractDoctrineResult>(StatusCodes.Status200OK);
}
