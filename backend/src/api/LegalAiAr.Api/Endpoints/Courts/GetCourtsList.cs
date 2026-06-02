using LegalAiAr.Api.Interfaces;
using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Catalogs.Queries.GetCourts;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Courts;

public sealed class GetCourtsList : IEndpoint
{
    public string GroupName => CourtsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (
            string? q,
            string? jurisdictionArea,
            string? instance,
            int limit,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            limit = limit == 0 ? 50 : limit;
            var result = await mediator.Send(
                new GetCourtsQuery(q, jurisdictionArea, instance, limit),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetCourtsList")
        .WithTags("Courts")
        .Produces<IReadOnlyList<CourtListItemDto>>(StatusCodes.Status200OK);
}

internal static class CourtsEndpointGroup
{
    public const string Name = "courts";
}
