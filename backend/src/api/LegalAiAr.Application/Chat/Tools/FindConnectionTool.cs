using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Finds the shortest citation path between two rulings via recursive CTE.
/// Answers "how are these two cases related?" queries.
/// </summary>
public sealed class FindConnectionTool : IChatTool
{
    public string Name => "find_connection";

    public string Description =>
        "Find the shortest citation path between two rulings. Returns the chain of intermediate " +
        "cases connecting them. Use when the user asks how two cases are related or connected.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "sourceRulingId": { "type": "string", "format": "uuid", "description": "UUID of the starting ruling." },
            "targetRulingId": { "type": "string", "format": "uuid", "description": "UUID of the target ruling." }
          },
          "required": ["sourceRulingId", "targetRulingId"]
        }
        """;

    public async Task<string> ExecuteAsync(
        string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var root = doc.RootElement;

        if (!root.TryGetProperty("sourceRulingId", out var srcEl) ||
            !Guid.TryParse(srcEl.GetString(), out var sourceId))
            return "Error: Invalid sourceRulingId format.";

        if (!root.TryGetProperty("targetRulingId", out var tgtEl) ||
            !Guid.TryParse(tgtEl.GetString(), out var targetId))
            return "Error: Invalid targetRulingId format.";

        var graph = context.Services.GetRequiredService<IGraphService>();
        var path = await graph.GetCitationPathAsync(sourceId, targetId, 5, cancellationToken);

        if (path is null || path.Count == 0)
            return $"[find_connection: no path found] No citation path found between {sourceId} and {targetId} within 5 hops.";

        var sb = new StringBuilder();
        sb.AppendLine($"[find_connection: {path.Count} steps]");
        sb.AppendLine();

        for (var i = 0; i < path.Count; i++)
        {
            var step = path[i];
            var arrow = i == 0 ? "START" : $"──({step.CitationType ?? "CITES"})──►";
            sb.AppendLine($"  {arrow} [{step.StepIndex}] {step.CaseTitle} | ID: {step.RulingId} | {step.RulingDate:yyyy-MM-dd} | Court: {step.CourtName ?? "N/A"}");
            context.ResolvedRulingIds.Add(step.RulingId);
        }

        sb.AppendLine();
        sb.AppendLine($"Path length: {path.Count - 1} citation hop(s).");
        return sb.ToString();
    }
}
