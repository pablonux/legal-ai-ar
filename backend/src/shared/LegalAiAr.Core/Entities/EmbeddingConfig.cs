namespace LegalAiAr.Core.Entities;

/// <summary>
/// Versioned configuration for embeddings and chunking strategy.
/// When the active config changes, rulings are flagged for re-embedding.
/// </summary>
public class EmbeddingConfig
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = string.Empty;
    public int EmbeddingDimensions { get; set; }
    public string ContextualizationMethod { get; set; } = string.Empty;
    public string ChunkingStrategy { get; set; } = string.Empty;
    public int ChunkSize { get; set; }
    public int ChunkOverlap { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public ICollection<RulingEmbeddingState> RulingStates { get; set; } = new List<RulingEmbeddingState>();
    public ICollection<GraphCommunity> Communities { get; set; } = new List<GraphCommunity>();
}
