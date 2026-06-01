namespace LegalAiAr.Core.Models;

/// <summary>
/// Input for indexing a chunk in Azure AI Search rulings-by-chunk index.
/// </summary>
public record ChunkIndexInput(
    Guid RulingId,
    int ChunkIndex,
    string Text,
    string ContextualizedText,
    float[] Embedding);
