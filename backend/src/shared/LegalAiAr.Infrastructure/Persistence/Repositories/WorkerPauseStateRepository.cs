using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal sealed class WorkerPauseStateRepository : IWorkerPauseStateRepository
{
    private readonly AppDbContext _context;

    public WorkerPauseStateRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<WorkerPauseState>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WorkerPauseStates
            .AsNoTracking()
            .OrderBy(w => w.WorkerType)
            .ToListAsync(cancellationToken);
    }
}
