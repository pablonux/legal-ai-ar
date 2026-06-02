using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;
using LegalAiAr.Application.Thesaurus.Queries.SearchThesaurus;

namespace LegalAiAr.Api.Endpoints.Thesaurus;

public sealed class GetThesaurusSearch : IEndpoint
{
    public string GroupName => ThesaurusEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/search", async (
            string q,
            int limit,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
                return Results.Ok(Array.Empty<ThesaurusTermDto>());

            var result = await mediator.Send(new SearchThesaurusQuery(q.Trim(), limit == 0 ? 10 : limit), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetThesaurusSearch")
        .WithTags("Thesaurus")
        .Produces<IReadOnlyList<ThesaurusTermDto>>(StatusCodes.Status200OK);
}

internal static class ThesaurusEndpointGroup
{
    public const string Name = "thesaurus";
}
