using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Stats.DTOs;
using LegalAiAr.Application.Stats.Queries.GetKbStats;

namespace LegalAiAr.Api.Endpoints.Stats;

public sealed class GetKbStats : IEndpoint
{
    public string GroupName => StatsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/kb", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetKbStatsQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetKbStats")
        .WithTags("Stats")
        .Produces<KbStatsDto>(StatusCodes.Status200OK);
}

internal static class StatsEndpointGroup
{
    public const string Name = "stats";
}
