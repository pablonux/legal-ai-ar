using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Crawlers.DTOs;
using LegalAiAr.Application.Admin.Crawlers.Queries.GetCrawlers;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Crawlers;

public sealed class GetAdminCrawlerById : IEndpoint
{
    public string GroupName => AdminCrawlersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{sourceId:int}", async (
            int sourceId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetCrawlersQuery(SourceId: sourceId), cancellationToken);
            return Results.Ok(result[0]);
        })
        .WithName("GetAdminCrawlerById")
        .WithTags("AdminCrawlers")
        .Produces<CrawlerConfigDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
