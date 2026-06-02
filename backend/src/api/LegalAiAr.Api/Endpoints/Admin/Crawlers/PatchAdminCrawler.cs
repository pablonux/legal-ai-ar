using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Crawlers.Commands.UpdateCrawlerConfig;
using LegalAiAr.Application.Admin.Crawlers.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Crawlers;

public sealed class PatchAdminCrawler : IEndpoint
{
    public string GroupName => AdminCrawlersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPatch("/{sourceId:int}", async (
            int sourceId,
            UpdateCrawlerConfigRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (request is null)
                return Results.BadRequest(new { error = "Request body is required." });

            var result = await mediator.Send(
                new UpdateCrawlerConfigCommand(sourceId, request.IsEnabled),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PatchAdminCrawler")
        .WithTags("AdminCrawlers")
        .Produces<CrawlerConfigDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
}
