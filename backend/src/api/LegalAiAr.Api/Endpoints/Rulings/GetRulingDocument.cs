using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Rulings.Queries.GetRulingDocument;

namespace LegalAiAr.Api.Endpoints.Rulings;

public sealed class GetRulingDocument : IEndpoint
{
    public string GroupName => RulingsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:guid}/document", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetRulingDocumentQuery(id), cancellationToken);
            return result is null
                ? Results.NotFound()
                : Results.File(result.Content, result.ContentType, enableRangeProcessing: true);
        })
        .WithName("GetRulingDocument")
        .WithTags("Rulings")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}
