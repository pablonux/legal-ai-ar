namespace LegalAiAr.Core.Entities;

/// <summary>
/// Tracks the embedding state of each ruling relative to the active EmbeddingConfig.
/// When a new config is activated, rulings with a different config are flagged for re-embedding.
/// </summary>
public class RulingEmbeddingState
{
    public Guid RulingId { get; set; }
    public int EmbeddingConfigId { get; set; }
    public DateTime EmbeddedAt { get; set; }
    public int ChunkCount { get; set; }
    public bool NeedsReembedding { get; set; }

    public Ruling Ruling { get; set; } = null!;
    public EmbeddingConfig EmbeddingConfig { get; set; } = null!;
}
