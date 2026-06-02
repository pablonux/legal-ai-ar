using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobMetrics;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class GetJobMetrics : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/jobs/{id:guid}/metrics", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetJobMetricsQuery(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetJobMetrics")
        .WithTags("AdminJobs");
}
