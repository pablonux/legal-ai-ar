namespace LegalAiAr.Core.Entities;

/// <summary>
/// Assigns an entity to a GraphRAG community with a relevance score.
/// </summary>
public class CommunityMembership
{
    public int Id { get; set; }
    public int CommunityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public float? Relevance { get; set; }

    public GraphCommunity Community { get; set; } = null!;
}
