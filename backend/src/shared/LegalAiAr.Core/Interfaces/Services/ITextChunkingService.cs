using LegalAiAr.Core.Messages;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Chunks full text into segments for indexing (ADR-002, E059).
/// Used by RequeueDocument for indexer reconstruction.
/// </summary>
public interface ITextChunkingService
{
    /// <summary>
    /// Splits fullText into 512-token chunks with 50-token overlap.
    /// </summary>
    IReadOnlyList<ChunkData> Chunk(string fullText);
}
