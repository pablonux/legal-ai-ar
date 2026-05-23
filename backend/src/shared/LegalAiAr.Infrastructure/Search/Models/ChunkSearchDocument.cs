using System.Text.Json.Serialization;

namespace LegalAiAr.Infrastructure.Search.Models;

/// <summary>
/// Document model for the rulings-by-chunk Azure AI Search index.
/// Matches the schema created by LegalAiAr.SetupSearch.
/// </summary>
public class ChunkSearchDocument
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("rulingId")]
    public string RulingId { get; set; } = string.Empty;

    [JsonPropertyName("chunkIndex")]
    public int ChunkIndex { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("contextualizedText")]
    public string? ContextualizedText { get; set; }

    [JsonPropertyName("embedding")]
    public IReadOnlyList<float>? Embedding { get; set; }
}
