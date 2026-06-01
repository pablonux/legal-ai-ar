using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Finds rulings semantically similar to a given ruling.
/// </summary>
public sealed class GetRelatedRulingsTool : IChatTool
{
    public string Name => "get_related_rulings";

    public string Description =>
        "Find rulings semantically similar to a given ruling. Use when the user asks for related or similar jurisprudence.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "rulingId": { "type": "string", "format": "uuid", "description": "UUID of the reference ruling." },
            "topK": { "type": "integer", "description": "Max results (1-10). Default: 5." }
          },
          "required": ["rulingId"]
        }
        """;

    public async Task<string> ExecuteAsync(
        string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var root = doc.RootElement;

        if (!root.TryGetProperty("rulingId", out var idEl) ||
            !Guid.TryParse(idEl.GetString(), out var rulingId))
            return "Error: Invalid ruling ID format.";

        var topK = Math.Clamp(
            root.TryGetProperty("topK", out var topKEl) && topKEl.TryGetInt32(out var k) ? k : 5,
            1, 10);

        var search = context.Services.GetRequiredService<ISearchService>();
        var results = await search.SearchRelatedAsync(rulingId, topK, cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine($"[get_related_rulings: {results.Count} results for ruling {rulingId}]");

        if (results.Count == 0)
        {
            sb.AppendLine("No related rulings found.");
            return sb.ToString();
        }

        sb.AppendLine();
        for (var i = 0; i < results.Count; i++)
        {
            var r = results[i];
            sb.AppendLine($"{i + 1}. Case: {r.CaseTitle} | ID: {r.RulingId} | Date: {r.RulingDate:yyyy-MM-dd} | Score: {r.Score:F3}");
            sb.AppendLine($"   Court: {r.Court ?? "N/A"} | Summary: {r.Summary ?? "N/A"}");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
