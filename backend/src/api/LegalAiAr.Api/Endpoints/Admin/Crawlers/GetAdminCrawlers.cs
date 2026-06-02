using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Crawlers.DTOs;
using LegalAiAr.Application.Admin.Crawlers.Queries.GetCrawlers;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Crawlers;

public sealed class GetAdminCrawlers : IEndpoint
{
    public string GroupName => AdminCrawlersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetCrawlersQuery(SourceId: null), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetAdminCrawlers")
        .WithTags("AdminCrawlers")
        .Produces<IReadOnlyList<CrawlerConfigDto>>(StatusCodes.Status200OK);
}

internal static class AdminCrawlersEndpointGroup
{
    public const string Name = "admin/crawlers";
}
