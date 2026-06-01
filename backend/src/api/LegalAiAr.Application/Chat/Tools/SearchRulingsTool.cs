using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Searches jurisprudential rulings with optional filters via hybrid semantic search.
/// Primary tool — the model should invoke this for most jurisprudential queries.
/// </summary>
public sealed class SearchRulingsTool : IChatTool
{
    private const int DefaultTopK = 5;
    private const int MaxTopK = 20;

    public string Name => "search_rulings";

    public string Description =>
        "Search jurisprudential rulings with optional filters. Returns ruling metadata and summaries. Use for any query about rulings matching specific criteria.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "query": { "type": "string", "description": "Natural language search query describing the legal topic or question." },
            "dateFrom": { "type": "string", "format": "date", "description": "Earliest ruling date (YYYY-MM-DD)." },
            "dateTo": { "type": "string", "format": "date", "description": "Latest ruling date (YYYY-MM-DD)." },
            "jurisdictionArea": { "type": "string", "description": "Jurisdiction area (e.g. Penal, Civil, Laboral, Contencioso Administrativo)." },
            "instance": { "type": "string", "description": "Court instance (e.g. CSJN, Camara, Primera Instancia)." },
            "courtName": { "type": "string", "description": "Court name." },
            "keywords": { "type": "array", "items": { "type": "string" }, "description": "Keyword descriptions to filter by." },
            "actionType": { "type": "string", "description": "Filter by action type (e.g. RECURSO EXTRAORDINARIO FEDERAL, QUEJA, COMPETENCIA)." },
            "officialReference": { "type": "string", "description": "Filter or search by official reference (e.g. 'Fallos: 340:1256')." },
            "hasDictamen": { "type": "boolean", "description": "Filter by presence of prosecutor opinion (dictamen)." },
            "topK": { "type": "integer", "description": "Max results to return (1-20). Default: 5." }
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

        var filters = new SearchFilters(
            JurisdictionArea: GetOptionalString(root, "jurisdictionArea"),
            Instance: GetOptionalString(root, "instance"),
            CourtName: GetOptionalString(root, "courtName"),
            DateFrom: GetOptionalDate(root, "dateFrom"),
            DateTo: GetOptionalDate(root, "dateTo"),
            Keywords: GetOptionalStringArray(root, "keywords"),
            ActionType: GetOptionalString(root, "actionType"),
            OfficialReference: GetOptionalString(root, "officialReference"),
            HasDictamen: GetOptionalBool(root, "hasDictamen"));

        var embeddings = context.Services.GetRequiredService<IEmbeddingService>();
        var search = context.Services.GetRequiredService<ISearchService>();

        var embedding = await embeddings.GenerateAsync(query, cancellationToken);
        var result = await search.SearchAsync(embedding, query, filters, 1, topK, cancellationToken);

        foreach (var item in result.Items)
            context.ResolvedRulingIds.Add(item.RulingId);

        return FormatResults(query, result.Items);
    }

    private static string FormatResults(string query, IReadOnlyList<SearchResultItem> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[search_rulings: {items.Count} results for \"{query}\"]");

        if (items.Count == 0)
        {
            sb.AppendLine("No rulings found matching the criteria.");
            return sb.ToString();
        }

        sb.AppendLine();
        for (var i = 0; i < items.Count; i++)
        {
            var r = items[i];
            sb.AppendLine($"{i + 1}. Case: {r.CaseTitle} | ID: {r.RulingId} | Date: {r.RulingDate:yyyy-MM-dd}");
            sb.AppendLine($"   Court: {r.Court ?? "N/A"} | Area: {r.JurisdictionArea ?? "N/A"} / {r.Instance ?? "N/A"}");
            if (!string.IsNullOrWhiteSpace(r.Summary))
                sb.AppendLine($"   Summary: {r.Summary}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string? GetOptionalString(JsonElement root, string property) =>
        root.TryGetProperty(property, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()
            : null;

    private static DateOnly? GetOptionalDate(JsonElement root, string property)
    {
        if (!root.TryGetProperty(property, out var el) || el.ValueKind != JsonValueKind.String)
            return null;
        return DateOnly.TryParse(el.GetString(), out var d) ? d : null;
    }

    private static bool? GetOptionalBool(JsonElement root, string property)
    {
        if (!root.TryGetProperty(property, out var el))
            return null;
        return el.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null
        };
    }

    private static IReadOnlyList<string>? GetOptionalStringArray(JsonElement root, string property)
    {
        if (!root.TryGetProperty(property, out var el) || el.ValueKind != JsonValueKind.Array)
            return null;
        var list = new List<string>();
        foreach (var item in el.EnumerateArray())
        {
            var s = item.GetString();
            if (!string.IsNullOrWhiteSpace(s))
                list.Add(s);
        }
        return list.Count > 0 ? list : null;
    }
}
