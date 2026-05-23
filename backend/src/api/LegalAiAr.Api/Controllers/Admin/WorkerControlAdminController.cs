using LegalAiAr.Api.Hubs;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using LegalAiAr.Core.Entities;

namespace LegalAiAr.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/workers")]
[Authorize]
public class WorkerControlAdminController : ControllerBase
{
    private readonly IHubContext<WorkerControlHub, IWorkerControlClient> _hubContext;
    private readonly AppDbContext _db;

    public WorkerControlAdminController(
        IHubContext<WorkerControlHub, IWorkerControlClient> hubContext,
        AppDbContext db)
    {
        _hubContext = hubContext;
        _db = db;
    }

    [HttpPost("{workerType}/pause")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PauseWorker(string workerType, CancellationToken cancellationToken)
    {
        await UpsertPauseStateAsync(workerType, isPaused: true, cancellationToken);
        await _hubContext.Clients.Group(workerType).PauseAsync();
        return Ok(new { workerType, isPaused = true });
    }

    [HttpPost("{workerType}/resume")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResumeWorker(string workerType, CancellationToken cancellationToken)
    {
        await UpsertPauseStateAsync(workerType, isPaused: false, cancellationToken);
        await _hubContext.Clients.Group(workerType).ResumeAsync();
        return Ok(new { workerType, isPaused = false });
    }

    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStatus(CancellationToken cancellationToken)
    {
        var states = await _db.WorkerPauseStates.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(states);
    }

    private async Task UpsertPauseStateAsync(string workerType, bool isPaused, CancellationToken ct)
    {
        var existing = await _db.WorkerPauseStates.FindAsync([workerType], ct);
        if (existing is not null)
        {
            existing.IsPaused = isPaused;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.WorkerPauseStates.Add(new WorkerPauseState
            {
                WorkerType = workerType,
                IsPaused = isPaused,
                UpdatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync(ct);
    }
}
