using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;
using LegalAiAr.Application.Thesaurus.Queries.GetThesaurusById;

namespace LegalAiAr.Api.Endpoints.Thesaurus;

public sealed class GetThesaurusById : IEndpoint
{
    public string GroupName => ThesaurusEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetThesaurusByIdQuery(id), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetThesaurusById")
        .WithTags("Thesaurus")
        .Produces<ThesaurusTermDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
