using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;
using LegalAiAr.Application.Thesaurus.Queries.GetThesaurusRoots;

namespace LegalAiAr.Api.Endpoints.Thesaurus;

public sealed class GetThesaurusRoots : IEndpoint
{
    public string GroupName => ThesaurusEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/roots", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetThesaurusRootsQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetThesaurusRoots")
        .WithTags("Thesaurus")
        .Produces<IReadOnlyList<ThesaurusTermDto>>(StatusCodes.Status200OK);
}
