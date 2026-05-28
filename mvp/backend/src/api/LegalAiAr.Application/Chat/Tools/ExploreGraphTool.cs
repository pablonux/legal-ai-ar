using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Explores the graph neighborhood of a ruling: persons, keywords, statutes, court,
/// plus cited and citing rulings. Provides structural context for GraphRAG.
/// </summary>
public sealed class ExploreGraphTool : IChatTool
{
    public string Name => "explore_graph";

    public string Description =>
        "Explore the full graph context of a ruling: its persons (judges, prosecutors), keywords, cited statutes, court, " +
        "plus rulings it cites and rulings that cite it. Use when you need to understand a ruling's " +
        "legal context, precedent relationships, or connected entities.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "rulingId": { "type": "string", "format": "uuid", "description": "UUID of the ruling to explore." }
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

        var graph = context.Services.GetRequiredService<IGraphService>();
        var neighborhood = await graph.GetGraphNeighborhoodAsync(rulingId, cancellationToken);

        if (neighborhood is null)
            return $"[explore_graph: not found] No ruling found with ID {rulingId}.";

        context.ResolvedRulingIds.Add(rulingId);

        var sb = new StringBuilder();
        sb.AppendLine($"[explore_graph: {neighborhood.CaseTitle}]");
        sb.AppendLine();
        sb.AppendLine($"Center: {neighborhood.CaseTitle} | ID: {neighborhood.CenterRulingId} | Date: {neighborhood.RulingDate:yyyy-MM-dd} | Court: {neighborhood.CourtName ?? "N/A"}");
        sb.AppendLine();

        if (neighborhood.Persons.Count > 0)
        {
            sb.AppendLine($"Persons ({neighborhood.Persons.Count}):");
            foreach (var p in neighborhood.Persons)
                sb.AppendLine($"  - {p.FullName} ({p.RulingRole})");
            sb.AppendLine();
        }

        if (neighborhood.Keywords.Count > 0)
        {
            sb.AppendLine($"Keywords ({neighborhood.Keywords.Count}):");
            foreach (var k in neighborhood.Keywords)
                sb.AppendLine($"  - {k.Description}");
            sb.AppendLine();
        }

        if (neighborhood.Statutes.Count > 0)
        {
            sb.AppendLine($"Cited Statutes ({neighborhood.Statutes.Count}):");
            foreach (var s in neighborhood.Statutes)
                sb.AppendLine($"  - {s.Number} {s.Name}{(s.Articles != null ? $" ({s.Articles})" : "")}");
            sb.AppendLine();
        }

        if (neighborhood.OutboundCitations.Count > 0)
        {
            sb.AppendLine($"Cites ({neighborhood.OutboundCitations.Count} rulings):");
            foreach (var c in neighborhood.OutboundCitations)
            {
                sb.AppendLine($"  - {c.CaseTitle} | ID: {c.RulingId} | {c.RulingDate:yyyy-MM-dd} | {c.CitationType}");
                context.ResolvedRulingIds.Add(c.RulingId);
            }
            sb.AppendLine();
        }

        if (neighborhood.InboundCitations.Count > 0)
        {
            sb.AppendLine($"Cited by ({neighborhood.InboundCitations.Count} rulings):");
            foreach (var c in neighborhood.InboundCitations)
            {
                sb.AppendLine($"  - {c.CaseTitle} | ID: {c.RulingId} | {c.RulingDate:yyyy-MM-dd} | {c.CitationType}");
                context.ResolvedRulingIds.Add(c.RulingId);
            }
        }

        return sb.ToString();
    }
}
