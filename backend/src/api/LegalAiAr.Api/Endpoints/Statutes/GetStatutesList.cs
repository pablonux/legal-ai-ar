using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Statutes;
using LegalAiAr.Application.Statutes.Queries.GetStatutes;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Api.Endpoints.Statutes;

public sealed class GetStatutesList : IEndpoint
{
    public string GroupName => StatutesEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (
            string? q,
            NormType? normType,
            NormativeLevel? normativeLevel,
            LegalBranch? legalBranch,
            bool? isVigente,
            int page,
            int pageSize,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            page = page == 0 ? 1 : page;
            pageSize = pageSize == 0 ? 25 : pageSize;
            var result = await mediator.Send(
                new GetStatutesQuery(q, normType, normativeLevel, legalBranch, isVigente, page, pageSize),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetStatutesList")
        .WithTags("Statutes")
        .Produces<StatutePageResponse>(StatusCodes.Status200OK);
}

internal static class StatutesEndpointGroup
{
    public const string Name = "statutes";
}
