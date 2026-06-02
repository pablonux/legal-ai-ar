using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Mapping;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Crawlers.Commands.RunCrawler;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Crawlers;

public sealed class PostAdminCrawlerRun : IEndpoint
{
    public string GroupName => AdminCrawlersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/{sourceId:int}/run", async (
            int sourceId,
            RunCrawlerRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var (command, error) = RunCrawlerRequestMapper.TryMap(sourceId, request);
            if (error is not null)
                return error;

            var result = await mediator.Send(command!, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostAdminCrawlerRun")
        .WithTags("AdminCrawlers")
        .Produces<RunCrawlerResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
}
