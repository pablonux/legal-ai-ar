using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Search;
using LegalAiAr.Application.Search.Queries;

namespace LegalAiAr.Api.Endpoints.Search;

public sealed class GetGlobalSearch : IEndpoint
{
    public string GroupName => SearchEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (
            string q,
            int maxPerEntity,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var limit = maxPerEntity == 0 ? 5 : maxPerEntity;
            var result = await mediator.Send(new GlobalSearchQuery(q, limit), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetGlobalSearch")
        .WithTags("Search")
        .Produces<GlobalSearchResultDto>(StatusCodes.Status200OK);
}

internal static class SearchEndpointGroup
{
    public const string Name = "search";
}
