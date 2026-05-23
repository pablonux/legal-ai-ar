namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Detects communities of related entities from the KB graph
/// and persists them as GraphCommunity + CommunityMembership.
/// </summary>
public interface ICommunityDetectionService
{
    /// <summary>
    /// Runs community detection over the entire KB graph.
    /// Clears existing communities and rebuilds from scratch.
    /// Returns the number of communities created.
    /// </summary>
    Task<int> DetectCommunitiesAsync(CancellationToken cancellationToken = default);
}
