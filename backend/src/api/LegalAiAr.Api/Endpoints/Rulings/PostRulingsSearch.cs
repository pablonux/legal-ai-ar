using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Mapping;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Rulings.Queries.SearchRulings;
using LegalAiAr.Contracts.Requests.Rulings;
using LegalAiAr.Contracts.Responses.Rulings;

namespace LegalAiAr.Api.Endpoints.Rulings;

public sealed class PostRulingsSearch : IEndpoint
{
    public string GroupName => RulingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/search", async (
            SearchRulingsRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = SearchRulingsRequestMapper.ToQuery(request);
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostRulingsSearch")
        .WithTags("Rulings")
        .Produces<SearchRulingsResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
}

internal static class RulingsEndpointGroup
{
    public const string Name = "rulings";
}
