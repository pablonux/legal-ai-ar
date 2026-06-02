using LegalAiAr.Api.Interfaces;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Api.Endpoints.Admin.Workers;

public sealed class GetAdminWorkersStatus : IEndpoint
{
    public string GroupName => AdminWorkersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/status", async (AppDbContext db, CancellationToken cancellationToken) =>
        {
            var states = await db.WorkerPauseStates.AsNoTracking().ToListAsync(cancellationToken);
            return Results.Ok(states);
        })
        .WithName("GetAdminWorkersStatus")
        .WithTags("AdminWorkers")
        .Produces(StatusCodes.Status200OK);
}
