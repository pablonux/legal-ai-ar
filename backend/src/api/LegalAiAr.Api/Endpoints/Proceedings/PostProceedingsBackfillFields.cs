using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Commands;
using LegalAiAr.Application.Proceedings.Models;

namespace LegalAiAr.Api.Endpoints.Proceedings;

public sealed class PostProceedingsBackfillFields : IEndpoint
{
    public string GroupName => ProceedingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/backfill-fields", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new BackfillProceedingFieldsCommand(), cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization("AdminOnly")
        .WithName("PostProceedingsBackfillFields")
        .WithTags("Proceedings")
        .Produces<BackfillProceedingFieldsResult>(StatusCodes.Status200OK);
}
