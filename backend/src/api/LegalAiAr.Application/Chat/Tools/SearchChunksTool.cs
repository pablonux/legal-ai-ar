using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Searches text passages (chunks) within rulings using hybrid search (vector + BM25).
/// Complements search_rulings (summaries) with passage-level retrieval for specific
/// legal reasoning, exact quotes, statute references, or argument details.
/// </summary>
public sealed class SearchChunksTool : IChatTool
{
    private const int DefaultTopK = 5;
    private const int MaxTopK = 15;

    public string Name => "search_chunks";

    public string Description =>
        "Search specific text passages within rulings using hybrid search. Returns relevant text fragments with ruling context. " +
        "Use when you need exact legal reasoning, specific quotes, statute article interpretation, or detailed argument passages — " +
        "not just summaries. Can optionally filter to a single ruling.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "query": { "type": "string", "description": "Natural language query or specific text to search for (e.g. statute number, legal concept, quote fragment)." },
            "rulingId": { "type": "string", "format": "uuid", "description": "Optional: restrict search to chunks of a specific ruling." },
            "topK": { "type": "integer", "description": "Max passages to return (1-15). Default: 5." }
          },
          "required": ["query"]
        }
        """;

    public async Task<string> ExecuteAsync(
        string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var root = doc.RootElement;

        var query = root.GetProperty("query").GetString();
        if (string.IsNullOrWhiteSpace(query))
            return "Error: query is required.";

        var topK = Math.Clamp(
            root.TryGetProperty("topK", out var topKEl) && topKEl.TryGetInt32(out var k) ? k : DefaultTopK,
            1, MaxTopK);

        Guid? rulingId = null;
        if (root.TryGetProperty("rulingId", out var ridEl) &&
            Guid.TryParse(ridEl.GetString(), out var parsedRid))
            rulingId = parsedRid;

        var embeddings = context.Services.GetRequiredService<IEmbeddingService>();
        var search = context.Services.GetRequiredService<ISearchService>();

        var embedding = await embeddings.GenerateAsync(query, cancellationToken);
        var chunks = await search.SearchChunksAsync(embedding, topK, query, rulingId, cancellationToken);

        foreach (var chunk in chunks)
            context.ResolvedRulingIds.Add(chunk.RulingId);

        return FormatResults(query, chunks, rulingId);
    }

    private static string FormatResults(string query, IReadOnlyList<ChatChunkResult> chunks, Guid? rulingId)
    {
        var sb = new StringBuilder();
        var scope = rulingId.HasValue ? $" within ruling {rulingId.Value}" : "";
        sb.AppendLine($"[search_chunks: {chunks.Count} passages for \"{query}\"{scope}]");

        if (chunks.Count == 0)
        {
            sb.AppendLine("No relevant text passages found.");
            return sb.ToString();
        }

        sb.AppendLine();
        for (var i = 0; i < chunks.Count; i++)
        {
            var c = chunks[i];
            sb.AppendLine($"--- Passage {i + 1} (Ruling: {c.RulingId}, Chunk #{c.ChunkIndex}, Score: {c.Score:F3}) ---");
            sb.AppendLine(c.Text);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
