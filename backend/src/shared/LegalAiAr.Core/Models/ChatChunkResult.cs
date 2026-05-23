namespace LegalAiAr.Core.Models;

/// <summary>
/// Chunk result from rulings-by-chunk index for RAG context construction.
/// </summary>
/// <param name="RulingId">ID of the ruling the chunk belongs to.</param>
/// <param name="ChunkIndex">Index of the chunk within the ruling.</param>
/// <param name="Text">Chunk text content.</param>
/// <param name="Score">Relevance score.</param>
public record ChatChunkResult(
    Guid RulingId,
    int ChunkIndex,
    string Text,
    double Score);
