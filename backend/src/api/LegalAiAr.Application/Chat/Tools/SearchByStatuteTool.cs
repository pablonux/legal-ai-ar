using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

public sealed class SearchByStatuteTool : IChatTool
{
    private const int DefaultTopK = 10;
    private const int MaxTopK = 20;

    public string Name => "search_by_statute";

    public string Description =>
        "Find rulings that cite a specific law or statute. Use when the user asks about rulings applying or interpreting a particular legal norm.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "statuteName": { "type": "string", "description": "Name or description of the statute/law. Supports partial match." },
            "statuteNumber": { "type": "string", "description": "Official number of the statute (e.g. 26.994, 24.240)." },
            "articles": { "type": "string", "description": "Specific articles cited (e.g. art. 75 inc. 22). Narrows results." },
            "topK": { "type": "integer", "description": "Max results (1-20). Default: 10." }
          },
          "required": ["statuteName"]
        }
        """;

    public async Task<string> ExecuteAsync(
        string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var root = doc.RootElement;

        var statuteName = GetOptionalString(root, "statuteName");
        if (string.IsNullOrWhiteSpace(statuteName))
            return "Error: statuteName is required.";

        var statuteNumber = GetOptionalString(root, "statuteNumber");
        var articles = GetOptionalString(root, "articles");
        var topK = Math.Clamp(
            root.TryGetProperty("topK", out var topKEl) && topKEl.TryGetInt32(out var k) ? k : DefaultTopK,
            1, MaxTopK);

        var statuteRepo = context.Services.GetRequiredService<IStatuteRepository>();
        var results = await statuteRepo.FindRulingsByStatuteAsync(
            statuteName, statuteNumber, articles, topK, cancellationToken);

        return FormatResults(statuteName, results);
    }

    private static string FormatResults(string statuteName, IReadOnlyList<StatuteRulingResult> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[search_by_statute: {items.Count} rulings citing \"{statuteName}\"]");

        if (items.Count == 0)
        {
            sb.AppendLine("No rulings found citing the specified statute.");
            return sb.ToString();
        }

        sb.AppendLine();
        for (var i = 0; i < items.Count; i++)
        {
            var r = items[i];
            sb.AppendLine($"{i + 1}. Case: {r.CaseTitle} | ID: {r.RulingId} | Date: {r.RulingDate:yyyy-MM-dd}");
            sb.AppendLine($"   Court: {r.CourtName ?? "N/A"}");
            sb.AppendLine($"   Statute: {r.StatuteName} (No. {r.StatuteNumber ?? "N/A"}) — Articles: {r.Articles ?? "N/A"}");
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
}
