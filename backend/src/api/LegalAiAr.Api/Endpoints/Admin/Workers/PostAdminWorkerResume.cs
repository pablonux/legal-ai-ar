using LegalAiAr.Api.Hubs;
using LegalAiAr.Api.Interfaces;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;

namespace LegalAiAr.Api.Endpoints.Admin.Workers;

public sealed class PostAdminWorkerResume : IEndpoint
{
    public string GroupName => AdminWorkersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/{workerType}/resume", async (
            string workerType,
            IHubContext<WorkerControlHub, IWorkerControlClient> hubContext,
            AppDbContext db,
            CancellationToken cancellationToken) =>
        {
            await WorkerPauseStateService.UpsertPauseStateAsync(db, workerType, isPaused: false, cancellationToken);
            await hubContext.Clients.Group(workerType).ResumeAsync();
            return Results.Ok(new { workerType, isPaused = false });
        })
        .WithName("PostAdminWorkerResume")
        .WithTags("AdminWorkers")
        .Produces(StatusCodes.Status200OK);
}
