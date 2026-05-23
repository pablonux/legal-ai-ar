using System.ClientModel;
using System.Text.Json;
using Azure.AI.OpenAI;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace LegalAiAr.Infrastructure.Ai;

/// <summary>
/// LLM fallback enricher using GPT-4o-mini for queries that the rule-based
/// enricher couldn't classify with sufficient confidence.
/// </summary>
public sealed class AzureOpenAiQueryEnricher : IQueryEnricher
{
    private const string EnrichmentPrompt = """
        Analyze the following query for a legal jurisprudence search system. Return a JSON object with:
        - "intent": one of "search", "detail", "comparison", "statistics", "precedent_exploration", "statute_research", "general"
        - "entities": object with arrays for each category found:
          - "temporal": date ranges or years mentioned (e.g., [{{"from": 2020, "to": 2024}}])
          - "courts": court names or abbreviations (e.g., ["CSJN", "Cámara Federal"])
          - "statutes": laws or articles (e.g., [{{"statute": "CN", "articles": ["14"]}}])
          - "cases": case names (e.g., ["Ekmekdjian c/ Sofovich"])
          - "topics": legal topics or keywords (e.g., ["libertad de expresión"])

        Query: {0}

        Respond ONLY with the JSON object, no explanation.
        """;

    private readonly ChatClient _chatClient;
    private readonly ILogger<AzureOpenAiQueryEnricher> _logger;

    public AzureOpenAiQueryEnricher(
        IOptions<AzureOpenAiOptions> options,
        ILogger<AzureOpenAiQueryEnricher> logger)
    {
        _logger = logger;
        var opts = options.Value;
        var client = new AzureOpenAIClient(
            new Uri(opts.Endpoint),
            new System.ClientModel.ApiKeyCredential(opts.ApiKey));
        _chatClient = client.GetChatClient(opts.NanoDeploymentName);
    }

    public async Task<QueryEnrichment?> EnrichAsync(string query, CancellationToken cancellationToken = default)
    {
        var prompt = string.Format(EnrichmentPrompt, query);
        var chatOptions = new ChatCompletionOptions
        {
            Temperature = 0f,
            MaxOutputTokenCount = 200,
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
        };

        var messages = new List<ChatMessage> { new UserChatMessage(prompt) };

        ClientResult<ChatCompletion> result = await _chatClient.CompleteChatAsync(
            messages, chatOptions, cancellationToken);

        var json = result.Value.Content[0].Text.Trim();
        _logger.LogDebug("Nano query enrichment response: {Json}", json);

        return ParseEnrichmentJson(json);
    }

    private QueryEnrichment? ParseEnrichmentJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var intent = root.TryGetProperty("intent", out var intentEl)
                ? intentEl.GetString() ?? "general"
                : "general";

            var enrichment = new QueryEnrichment
            {
                Intent = intent,
                Source = EnrichmentSource.LlmFallback,
                Confidence = 0.80f,
            };

            if (!root.TryGetProperty("entities", out var entities))
                return enrichment;

            var temporal = new List<TemporalEntity>();
            if (entities.TryGetProperty("temporal", out var temporalArr) && temporalArr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in temporalArr.EnumerateArray())
                {
                    var from = item.TryGetProperty("from", out var f) ? f.GetInt32() : 0;
                    var to = item.TryGetProperty("to", out var t) ? (int?)t.GetInt32() : null;
                    temporal.Add(new TemporalEntity(from, to));
                }
            }

            var courts = new List<string>();
            if (entities.TryGetProperty("courts", out var courtsArr) && courtsArr.ValueKind == JsonValueKind.Array)
                foreach (var item in courtsArr.EnumerateArray())
                    if (item.GetString() is { } c) courts.Add(c);

            var statutes = new List<StatuteEntity>();
            if (entities.TryGetProperty("statutes", out var statutesArr) && statutesArr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in statutesArr.EnumerateArray())
                {
                    var statute = item.TryGetProperty("statute", out var s) ? s.GetString() ?? "" : "";
                    var articles = new List<string>();
                    if (item.TryGetProperty("articles", out var artArr) && artArr.ValueKind == JsonValueKind.Array)
                        foreach (var art in artArr.EnumerateArray())
                            if (art.GetString() is { } a) articles.Add(a);
                    statutes.Add(new StatuteEntity(statute, articles));
                }
            }

            var cases = new List<string>();
            if (entities.TryGetProperty("cases", out var casesArr) && casesArr.ValueKind == JsonValueKind.Array)
                foreach (var item in casesArr.EnumerateArray())
                    if (item.GetString() is { } cs) cases.Add(cs);

            var topics = new List<string>();
            if (entities.TryGetProperty("topics", out var topicsArr) && topicsArr.ValueKind == JsonValueKind.Array)
                foreach (var item in topicsArr.EnumerateArray())
                    if (item.GetString() is { } tp) topics.Add(tp);

            return new QueryEnrichment
            {
                Intent = enrichment.Intent,
                Source = enrichment.Source,
                Confidence = enrichment.Confidence,
                Temporal = temporal,
                Courts = courts,
                Statutes = statutes,
                Cases = cases,
                Topics = topics,
            };
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse LLM enrichment JSON");
            return null;
        }
    }
}
