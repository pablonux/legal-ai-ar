using LegalAiAr.Core.Messages;
using SharpToken;

namespace LegalAiAr.Worker.Indexer.Chunking;

/// <summary>
/// Chunks full text into 512-token segments with 50-token overlap (ADR-002, E059).
/// Uses cl100k_base encoding (GPT-4 / text-embedding-3-large compatible).
/// </summary>
public class TextChunkingService
{
    private const int ChunkSizeTokens = 512;
    private const int OverlapTokens = 50;
    private static readonly GptEncoding Encoding = GptEncoding.GetEncoding("cl100k_base");

    /// <summary>
    /// Splits fullText into chunks. Single short document emits one chunk. Empty input returns empty list.
    /// </summary>
    public IReadOnlyList<ChunkData> Chunk(string fullText)
    {
        if (string.IsNullOrWhiteSpace(fullText))
            return [];

        var tokens = Encoding.Encode(fullText);
        if (tokens.Count == 0)
            return [];

        if (tokens.Count <= ChunkSizeTokens)
            return [new ChunkData(0, fullText)];

        var chunks = new List<ChunkData>();
        var start = 0;
        var index = 0;

        while (start < tokens.Count)
        {
            var end = Math.Min(start + ChunkSizeTokens, tokens.Count);
            var chunkTokens = tokens.Skip(start).Take(end - start).ToList();
            var chunkText = Encoding.Decode(chunkTokens);
            chunks.Add(new ChunkData(index, chunkText));

            if (end >= tokens.Count)
                break;

            start = end - OverlapTokens;
            if (start < 0)
                start = 0;
            index++;
        }

        return chunks;
    }

    /// <summary>
    /// Returns chunks from the message if present; otherwise chunks FullText.
    /// Use when downstream steps need chunks and message may have empty Chunks.
    /// </summary>
    public IReadOnlyList<ChunkData> GetEffectiveChunks(IndexerMessage message)
    {
        if (message.Chunks.Count > 0)
            return message.Chunks;

        return Chunk(message.Ruling.FullText);
    }
}
