using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Commands;
using LegalAiAr.Application.Proceedings.Models;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class PostProceedingsExtractProsecutorOpinions : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/extract-prosecutor-opinions", async (
            int batchSize,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var size = batchSize == 0 ? 50 : batchSize;
            var result = await mediator.Send(new ExtractProsecutorOpinionsCommand(size), cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization("AdminOnly")
        .WithName("PostProceedingsExtractProsecutorOpinions")
        .WithTags("Proceedings")
        .Produces<ExtractProsecutorOpinionsResult>(StatusCodes.Status200OK);
}
