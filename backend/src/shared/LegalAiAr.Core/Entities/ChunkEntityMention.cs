using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Records which entities are mentioned in a specific text chunk of a ruling.
/// Enables GraphRAG local search: "found this chunk -> what entities are here? -> navigate graph".
/// High volume: ~10-20 mentions per chunk, ~20 chunks per ruling.
/// </summary>
public class ChunkEntityMention
{
    public long Id { get; set; }
    public Guid RulingId { get; set; }
    public int ChunkIndex { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public MentionType MentionType { get; set; }
    public float? Confidence { get; set; }

    public Ruling Ruling { get; set; } = null!;
}
