using LegalAiAr.Core.Entities;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Api.Endpoints.Admin.Workers;

internal static class WorkerPauseStateService
{
    public static async Task UpsertPauseStateAsync(
        AppDbContext db,
        string workerType,
        bool isPaused,
        CancellationToken cancellationToken)
    {
        var existing = await db.WorkerPauseStates.FindAsync([workerType], cancellationToken);
        if (existing is not null)
        {
            existing.IsPaused = isPaused;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            db.WorkerPauseStates.Add(new WorkerPauseState
            {
                WorkerType = workerType,
                IsPaused = isPaused,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
