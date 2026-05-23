using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

public sealed class ListCourtsTool : IChatTool
{
    private const int MaxResults = 50;

    public string Name => "list_courts";

    public string Description =>
        "List courts in the knowledge base. Use when the user asks about available courts or needs to discover court names for filtering.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "jurisdictionArea": { "type": "string", "description": "Filter by jurisdiction area." },
            "instance": { "type": "string", "description": "Filter by instance." }
          }
        }
        """;

    public async Task<string> ExecuteAsync(
        string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var root = doc.RootElement;

        var jurisdictionArea = GetOptionalString(root, "jurisdictionArea");
        var instance = GetOptionalString(root, "instance");

        var courtRepo = context.Services.GetRequiredService<ICourtRepository>();
        var courts = await courtRepo.ListAsync(jurisdictionArea, instance, MaxResults, cancellationToken);

        return FormatResults(courts);
    }

    private static string FormatResults(IReadOnlyList<Court> courts)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[list_courts: {courts.Count} courts]");

        if (courts.Count == 0)
        {
            sb.AppendLine("No courts found matching the criteria.");
            return sb.ToString();
        }

        sb.AppendLine();
        for (var i = 0; i < courts.Count; i++)
        {
            var c = courts[i];
            sb.AppendLine($"{i + 1}. {c.Name} | Area: {c.JurisdictionArea} | Territory: {c.Territory} | Instance: {c.Instance}");
        }

        return sb.ToString();
    }

    private static string? GetOptionalString(JsonElement root, string property) =>
        root.TryGetProperty(property, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()
            : null;
}
