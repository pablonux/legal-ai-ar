using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Commands.ReconcileJobCounters;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobReconcileCounters : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/reconcile-counters", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new ReconcileJobCountersCommand(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobReconcileCounters")
        .WithTags("AdminJobs");
}
