using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;
using LegalAiAr.Application.Thesaurus.Queries.GetThesaurusChildren;

namespace LegalAiAr.Api.Endpoints.Thesaurus;

public sealed class GetThesaurusChildren : IEndpoint
{
    public string GroupName => ThesaurusEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}/children", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetThesaurusChildrenQuery(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetThesaurusChildren")
        .WithTags("Thesaurus")
        .Produces<IReadOnlyList<ThesaurusTermDto>>(StatusCodes.Status200OK);
}
