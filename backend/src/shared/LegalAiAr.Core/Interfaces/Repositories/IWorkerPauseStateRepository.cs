using LegalAiAr.Core.Entities;

namespace LegalAiAr.Core.Interfaces.Repositories;

/// <summary>
/// Read-only access to persisted worker pause flags (shared DB row per worker type).
/// </summary>
public interface IWorkerPauseStateRepository
{
    Task<IReadOnlyList<WorkerPauseState>> GetAllAsync(CancellationToken cancellationToken = default);
}
