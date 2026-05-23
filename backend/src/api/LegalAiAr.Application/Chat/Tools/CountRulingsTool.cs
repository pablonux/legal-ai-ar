using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

public sealed class CountRulingsTool : IChatTool
{
    public string Name => "count_rulings";

    public string Description =>
        "Count rulings matching filters. Use for quantitative questions. Returns a count, not the rulings themselves.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "jurisdictionArea": { "type": "string", "description": "Filter by jurisdiction area." },
            "instance": { "type": "string", "description": "Filter by instance." },
            "courtName": { "type": "string", "description": "Filter by court name." },
            "dateFrom": { "type": "string", "format": "date", "description": "Earliest date." },
            "dateTo": { "type": "string", "format": "date", "description": "Latest date." },
            "isUnconstitutional": { "type": "boolean", "description": "Filter by unconstitutionality flag." },
            "keywords": { "type": "array", "items": { "type": "string" }, "description": "Filter by keywords." }
          }
        }
        """;

    public async Task<string> ExecuteAsync(
        string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var root = doc.RootElement;

        var filters = new CountFilters(
            JurisdictionArea: GetOptionalString(root, "jurisdictionArea"),
            Instance: GetOptionalString(root, "instance"),
            CourtName: GetOptionalString(root, "courtName"),
            DateFrom: GetOptionalDate(root, "dateFrom"),
            DateTo: GetOptionalDate(root, "dateTo"),
            IsUnconstitutional: GetOptionalBool(root, "isUnconstitutional"),
            Keywords: GetOptionalStringArray(root, "keywords"));

        var rulingRepo = context.Services.GetRequiredService<IRulingRepository>();
        var count = await rulingRepo.CountAsync(filters, cancellationToken);

        return FormatResult(count, filters);
    }

    private static string FormatResult(int count, CountFilters filters)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[count_rulings: {count} rulings]");

        var applied = new List<string>();
        if (!string.IsNullOrWhiteSpace(filters.JurisdictionArea))
            applied.Add($"jurisdictionArea={filters.JurisdictionArea}");
        if (!string.IsNullOrWhiteSpace(filters.Instance))
            applied.Add($"instance={filters.Instance}");
        if (!string.IsNullOrWhiteSpace(filters.CourtName))
            applied.Add($"courtName={filters.CourtName}");
        if (filters.DateFrom.HasValue)
            applied.Add($"dateFrom={filters.DateFrom.Value:yyyy-MM-dd}");
        if (filters.DateTo.HasValue)
            applied.Add($"dateTo={filters.DateTo.Value:yyyy-MM-dd}");
        if (filters.IsUnconstitutional.HasValue)
            applied.Add($"isUnconstitutional={filters.IsUnconstitutional.Value}");
        if (filters.Keywords is { Count: > 0 })
            applied.Add($"keywords=[{string.Join(", ", filters.Keywords)}]");

        sb.AppendLine(applied.Count > 0
            ? $"Filters applied: {string.Join(", ", applied)}"
            : "Filters applied: none (all rulings)");

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
