using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface ICourtRepository
{
    Task<Court?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Court> GetOrCreateAsync(string name, string jurisdictionArea, string territory, string instance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists courts with optional filters. Ordered by name, capped at <paramref name="maxResults"/>.
    /// </summary>
    Task<IReadOnlyList<Court>> ListAsync(
        string? jurisdictionArea = null,
        string? instance = null,
        int maxResults = 50,
        CancellationToken cancellationToken = default);

    /// <summary>Searches courts by name (LIKE). Returns courts with ruling count.</summary>
    Task<IReadOnlyList<CourtWithCount>> SearchAsync(
        string? query = null,
        string? jurisdictionArea = null,
        string? instance = null,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a court with its ruling count and participating judges.</summary>
    Task<CourtDetail?> GetByIdWithStatsAsync(int id, CancellationToken cancellationToken = default);
}
