using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Rulings.Queries.GetSearchFacets;
using LegalAiAr.Contracts.Responses.Rulings;

namespace LegalAiAr.Api.Endpoints.Rulings;

public sealed class GetRulingsFacets : IEndpoint
{
    public string GroupName => RulingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/facets", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var facets = await mediator.Send(new GetSearchFacetsQuery(), cancellationToken);
            return Results.Ok(facets);
        })
        .WithName("GetRulingsFacets")
        .WithTags("Rulings")
        .Produces<SearchFacetsResponse>(StatusCodes.Status200OK);
}
