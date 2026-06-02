using LegalAiAr.Api.Hubs;
using LegalAiAr.Api.Interfaces;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;

namespace LegalAiAr.Api.Endpoints.Admin.Workers;

public sealed class PostAdminWorkerPause : IEndpoint
{
    public string GroupName => AdminWorkersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/{workerType}/pause", async (
            string workerType,
            IHubContext<WorkerControlHub, IWorkerControlClient> hubContext,
            AppDbContext db,
            CancellationToken cancellationToken) =>
        {
            await WorkerPauseStateService.UpsertPauseStateAsync(db, workerType, isPaused: true, cancellationToken);
            await hubContext.Clients.Group(workerType).PauseAsync();
            return Results.Ok(new { workerType, isPaused = true });
        })
        .WithName("PostAdminWorkerPause")
        .WithTags("AdminWorkers")
        .Produces(StatusCodes.Status200OK);
}

internal static class AdminWorkersEndpointGroup
{
    public const string Name = "admin/workers";
}
