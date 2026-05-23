using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Worker.Indexer.Prompts;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Steps;

/// <summary>
/// Result of GenerateEmbeddingsStep: ruling-level and chunk-level embeddings (3072 dims each).
/// </summary>
/// <param name="RulingEmbedding">Embedding for summary+holding (ruling-level search).</param>
/// <param name="ChunkEmbeddings">Embeddings for each chunk, in index order.</param>
public record EmbeddingsResult(
    float[] RulingEmbedding,
    IReadOnlyList<ChunkWithEmbedding> ChunkEmbeddings);

/// <summary>
/// Chunk text with its embedding vector and contextualized text for BM25 indexing.
/// </summary>
public record ChunkWithEmbedding(int Index, string Text, string ContextualizedText, float[] Embedding);

/// <summary>
/// Generates embeddings for ruling level (summary+holding) and each chunk using text-embedding-3-large.
/// Supports rule-based (v1) and LLM-based (v2) chunk contextualization via EmbeddingConfig.
/// </summary>
public class GenerateEmbeddingsStep
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IEnrichmentService _enrichmentService;
    private readonly ILogger<GenerateEmbeddingsStep> _logger;

    public GenerateEmbeddingsStep(
        IEmbeddingService embeddingService,
        IEnrichmentService enrichmentService,
        ILogger<GenerateEmbeddingsStep> logger)
    {
        _embeddingService = embeddingService;
        _enrichmentService = enrichmentService;
        _logger = logger;
    }

    public async Task<EmbeddingsResult> ExecuteAsync(
        IndexerMessage message,
        IReadOnlyList<ChunkData> chunks,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(message, chunks, embeddingConfig: null, cancellationToken);
    }

    public async Task<EmbeddingsResult> ExecuteAsync(
        IndexerMessage message,
        IReadOnlyList<ChunkData> chunks,
        EmbeddingConfig? embeddingConfig,
        CancellationToken cancellationToken = default)
    {
        var ruling = message.Ruling;
        var rulingText = BuildRulingText(ruling.Summary, ruling.Holding, ruling.FullText, chunks);
        var useLlm = embeddingConfig?.ContextualizationMethod.StartsWith("llm", StringComparison.OrdinalIgnoreCase) == true;

        var chunkEntries = new List<(ChunkData Chunk, string ContextualizedText)>();
        for (var i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];
            if (string.IsNullOrWhiteSpace(chunk.Text))
                continue;

            string contextualizedText;
            if (useLlm)
            {
                contextualizedText = await ContextualizeWithLlmAsync(ruling, chunk.Text, cancellationToken)
                                     ?? BuildContextualizedChunkText(ruling, chunk.Text);
            }
            else
            {
                contextualizedText = BuildContextualizedChunkText(ruling, chunk.Text);
            }

            chunkEntries.Add((chunk, contextualizedText));
        }

        var allTexts = new List<string>(1 + chunkEntries.Count) { rulingText };
        foreach (var e in chunkEntries)
            allTexts.Add(e.ContextualizedText);

        _logger.LogDebug(
            "Generating embeddings (batch) for document {DocumentId}: ruling + {ChunkCount} chunks (method={Method})",
            message.DocumentId, chunkEntries.Count, useLlm ? "llm" : "rule-based");

        var allEmbeddings = await _embeddingService.GenerateBatchAsync(allTexts, cancellationToken);

        var rulingEmbedding = allEmbeddings[0];
        var chunkEmbeddings = new List<ChunkWithEmbedding>(chunkEntries.Count);
        for (var i = 0; i < chunkEntries.Count; i++)
        {
            var (chunk, contextualizedText) = chunkEntries[i];
            chunkEmbeddings.Add(new ChunkWithEmbedding(
                chunk.Index, chunk.Text, contextualizedText, allEmbeddings[i + 1]));
        }

        _logger.LogInformation(
            "Generated embeddings for document {DocumentId}: ruling + {ChunkCount} chunks (method={Method})",
            message.DocumentId, chunkEmbeddings.Count, useLlm ? "llm" : "rule-based");

        return new EmbeddingsResult(rulingEmbedding, chunkEmbeddings);
    }

    private async Task<string?> ContextualizeWithLlmAsync(
        RulingData ruling, string chunkText, CancellationToken ct)
    {
        try
        {
            var userPrompt = ChunkContextualizationPrompt.BuildUserPrompt(
                ruling.CaseTitle, ruling.Summary, ruling.Holding,
                ruling.Court ?? ruling.Instance, ruling.RulingDate, chunkText);

            var json = await _enrichmentService.GetStructuredOutputAsync(
                ChunkContextualizationPrompt.SystemPrompt,
                userPrompt,
                ChunkContextualizationPrompt.SchemaName,
                ChunkContextualizationPrompt.JsonSchema,
                ct);

            var doc = JsonDocument.Parse(json);
            var context = doc.RootElement.TryGetProperty("context", out var c) ? c.GetString() : null;
            if (string.IsNullOrWhiteSpace(context))
                return null;

            return $"{context} {chunkText}";
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "LLM contextualization failed for chunk, falling back to rule-based");
            return null;
        }
    }

    internal static string BuildContextualizedChunkText(RulingData ruling, string chunkText)
    {
        var parts = new List<string>(6);

        if (!string.IsNullOrWhiteSpace(ruling.Court))
            parts.Add(ruling.Court);
        else if (!string.IsNullOrWhiteSpace(ruling.Instance))
            parts.Add(ruling.Instance);

        parts.Add(ruling.RulingDate.ToString("yyyy-MM-dd"));

        if (!string.IsNullOrWhiteSpace(ruling.CaseTitle))
            parts.Add(ruling.CaseTitle.Length > 80 ? ruling.CaseTitle[..80] : ruling.CaseTitle);

        if (!string.IsNullOrWhiteSpace(ruling.SubjectArea))
            parts.Add(ruling.SubjectArea);

        if (!string.IsNullOrWhiteSpace(ruling.OfficialReference))
            parts.Add(ruling.OfficialReference);
        else if (!string.IsNullOrWhiteSpace(ruling.CaseNumber))
            parts.Add(ruling.CaseNumber);

        return $"[{string.Join(" | ", parts)}] {chunkText}";
    }

    private static string BuildRulingText(string? summary, string? holding, string fullText, IReadOnlyList<ChunkData> chunks)
    {
        var text = $"{summary ?? ""} {holding ?? ""}".Trim();
        if (!string.IsNullOrWhiteSpace(text))
            return text;

        if (chunks.Count > 0 && !string.IsNullOrWhiteSpace(chunks[0].Text))
            return chunks[0].Text;

        if (!string.IsNullOrWhiteSpace(fullText))
            return fullText.Length > 500 ? fullText[..500] : fullText;

        return "fallo";
    }
}
