using LegalAiAr.Core.Entities;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IGraphCommunityRepository
{
    Task<GraphCommunity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GraphCommunity>> GetByLevelAsync(int level, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GraphCommunity>> SearchByKeywordAsync(string keyword, int topK = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds communities that contain a specific entity.
    /// </summary>
    Task<IReadOnlyList<GraphCommunity>> GetCommunitiesForEntityAsync(
        string entityType, string entityId, CancellationToken cancellationToken = default);

    Task AddCommunityAsync(GraphCommunity community, CancellationToken cancellationToken = default);
    Task AddMembershipsAsync(IEnumerable<CommunityMembership> memberships, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all communities and memberships (for full rebuild).
    /// </summary>
    Task ClearAllAsync(CancellationToken cancellationToken = default);

    Task<int> GetCommunityCountAsync(CancellationToken cancellationToken = default);
}
