using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Returns full metadata for a specific ruling: court, persons, keywords, statutes, summary, holding.
/// </summary>
public sealed class GetRulingDetailTool : IChatTool
{
    public string Name => "get_ruling_detail";

    public string Description =>
        "Get full metadata for a specific ruling: court, judges, keywords, cited statutes, summary, holding. Use when the user asks for details about a specific ruling.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "rulingId": { "type": "string", "format": "uuid", "description": "UUID of the ruling to retrieve." }
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

        var repo = context.Services.GetRequiredService<IRulingRepository>();
        var ruling = await repo.GetByIdAsync(rulingId, cancellationToken);

        if (ruling is null)
            return $"[get_ruling_detail: not found] No ruling found with ID {rulingId}.";

        context.ResolvedRulingIds.Add(rulingId);

        var sb = new StringBuilder();
        sb.AppendLine($"[get_ruling_detail: {ruling.CaseTitle}]");
        sb.AppendLine();
        sb.AppendLine($"Case: {ruling.CaseTitle}");
        sb.AppendLine($"ID: {ruling.Id}");
        sb.AppendLine($"Date: {ruling.RulingDate:yyyy-MM-dd}");
        sb.AppendLine($"Court: {ruling.Court?.Name ?? "N/A"} ({ruling.JurisdictionArea ?? "N/A"} / {ruling.Instance ?? "N/A"})");
        sb.AppendLine($"Case Number: {ruling.CaseNumber ?? "N/A"}");
        sb.AppendLine($"Direction: {ruling.RulingDirection ?? "N/A"}");
        sb.AppendLine($"Subject Area: {ruling.SubjectArea ?? "N/A"}");
        if (!string.IsNullOrWhiteSpace(ruling.ActionType))
            sb.AppendLine($"Action Type: {ruling.ActionType}");
        if (!string.IsNullOrWhiteSpace(ruling.OfficialReference))
            sb.AppendLine($"Official Reference: {ruling.OfficialReference}");
        if (!string.IsNullOrWhiteSpace(ruling.FederalQuestion))
            sb.AppendLine($"Federal Question: {ruling.FederalQuestion}");
        if (!string.IsNullOrWhiteSpace(ruling.ProceduralFormula))
            sb.AppendLine($"Procedural Formula: {ruling.ProceduralFormula}");
        sb.AppendLine($"Unconstitutional: {ruling.IsUnconstitutional}");
        if (ruling.HasDictamen)
            sb.AppendLine("Has Prosecutor Opinion (Dictamen): Yes");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(ruling.Summary))
        {
            sb.AppendLine("Summary:");
            sb.AppendLine(ruling.Summary);
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(ruling.Holding))
        {
            sb.AppendLine("Holding:");
            sb.AppendLine(ruling.Holding);
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(ruling.Observations))
        {
            sb.AppendLine("Observations:");
            sb.AppendLine(ruling.Observations);
            sb.AppendLine();
        }

        if (ruling.RulingParticipations.Count > 0)
        {
            sb.AppendLine("Persons:");
            foreach (var rp in ruling.RulingParticipations)
                sb.AppendLine($"- {rp.Person.DisplayName} ({rp.Role})");
            sb.AppendLine();
        }

        if (ruling.RulingKeywords.Count > 0)
        {
            sb.AppendLine("Keywords:");
            foreach (var rk in ruling.RulingKeywords.OrderBy(k => k.SortOrder))
                sb.AppendLine($"- {rk.Keyword.Description}");
            sb.AppendLine();
        }

        if (ruling.RulingStatutes.Count > 0)
        {
            sb.AppendLine("Cited Statutes:");
            foreach (var rs in ruling.RulingStatutes)
                sb.AppendLine($"- {rs.Statute.Name} (No. {rs.Statute.Number}) — Articles: {rs.Articles ?? "N/A"}");
            sb.AppendLine();
        }

        if (ruling.OutboundCitations.Count > 0)
        {
            sb.AppendLine($"Outbound Citations ({ruling.OutboundCitations.Count}):");
            foreach (var c in ruling.OutboundCitations)
            {
                var target = c.TargetRuling is not null
                    ? $"{c.TargetRuling.CaseTitle} (ID: {c.TargetRulingId})"
                    : "unresolved";
                sb.AppendLine($"- {c.ExternalAlias} → {target} ({c.CitationType})");
            }
        }

        if (ruling.ProsecutorOpinion is { } po)
        {
            sb.AppendLine();
            sb.AppendLine("Prosecutor Opinion (Dictamen del Procurador):");
            sb.AppendLine($"- Prosecutor: {po.ProsecutorName}");
            if (!string.IsNullOrWhiteSpace(po.Summary))
                sb.AppendLine($"- Summary: {po.Summary}");
            if (!string.IsNullOrWhiteSpace(po.RecommendedDirection))
                sb.AppendLine($"- Recommended: {po.RecommendedDirection}");
            sb.AppendLine($"- Agreed with Court: {(po.AgreedWithCourt ? "Yes" : "No")}");
        }

        return sb.ToString();
    }
}
