using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Commands.RequeueFetcherPending;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobRequeueFetcherPending : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/requeue-fetcher-pending", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RequeueFetcherPendingCommand(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobRequeueFetcherPending")
        .WithTags("AdminJobs");
}
