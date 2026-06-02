using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Pipeline.Commands.BackfillKbQuality;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostPipelineBackfillKbQuality : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/pipeline/backfill-kb-quality", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new BackfillKbQualityCommand(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostPipelineBackfillKbQuality")
        .WithTags("AdminJobs");
}
