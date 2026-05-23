using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Returns the judicial proceeding chain for a ruling: all rulings in the same case
/// across court instances, ordered by instance level and date.
/// </summary>
public sealed class GetCaseHistoryTool : IChatTool
{
    public string Name => "get_case_history";

    public string Description =>
        "Get the full case history (proceeding chain) for a ruling: all related rulings in the same case across different court instances. Use when the user asks about appeal history, whether a ruling was upheld or overturned, or the procedural path of a case.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "rulingId": { "type": "string", "format": "uuid", "description": "UUID of the ruling to get case history for." }
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

        var repo = context.Services.GetRequiredService<IJudicialProceedingRepository>();
        var proceeding = await repo.GetByRulingIdAsync(rulingId, cancellationToken);

        if (proceeding is null)
            return $"[get_case_history: no proceeding] This ruling is not linked to a multi-instance judicial proceeding.";

        var sb = new StringBuilder();
        sb.AppendLine($"[get_case_history: {proceeding.CaseNumber}]");
        sb.AppendLine($"Case: {proceeding.DisplayName ?? proceeding.CaseNumber}");
        sb.AppendLine($"Jurisdiction: {proceeding.JurisdictionArea ?? "N/A"}");
        sb.AppendLine($"Total rulings in proceeding: {proceeding.RulingCount}");
        sb.AppendLine();

        sb.AppendLine("Instance chain (ordered by court level):");

        var rulings = proceeding.Rulings
            .OrderBy(r => r.Court?.InstanceLevel ?? 99)
            .ThenBy(r => r.RulingDate)
            .ToList();

        foreach (var r in rulings)
        {
            var marker = r.Id == rulingId ? " ← current" : "";
            sb.AppendLine($"- [{r.RulingDate:yyyy-MM-dd}] {r.Court?.Name ?? "N/A"} (Instance: {r.Court?.InstanceLevel ?? 0}) — {r.RulingDirection ?? "N/A"} — ID: {r.Id}{marker}");
            context.ResolvedRulingIds.Add(r.Id);
        }

        return sb.ToString();
    }
}
