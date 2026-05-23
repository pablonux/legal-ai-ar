namespace LegalAiAr.Core.Entities;

/// <summary>
/// Cluster of strongly related entities detected by GraphRAG community detection.
/// Hierarchical: leaf communities (Level 0) contain individual entities,
/// higher levels contain increasingly abstract clusters.
/// </summary>
public class GraphCommunity
{
    public int Id { get; set; }
    public int Level { get; set; }
    public int? ParentCommunityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? KeyFindings { get; set; }
    public int EntityCount { get; set; }
    public int? EmbeddingConfigId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public GraphCommunity? ParentCommunity { get; set; }
    public ICollection<GraphCommunity> ChildCommunities { get; set; } = new List<GraphCommunity>();
    public ICollection<CommunityMembership> Memberships { get; set; } = new List<CommunityMembership>();
    public EmbeddingConfig? EmbeddingConfig { get; set; }
}
