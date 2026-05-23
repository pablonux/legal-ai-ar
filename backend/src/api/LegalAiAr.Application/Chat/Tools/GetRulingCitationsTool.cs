using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Returns citation relationships for a ruling: inbound (cited by) and/or outbound (cites).
/// </summary>
public sealed class GetRulingCitationsTool : IChatTool
{
    public string Name => "get_ruling_citations";

    public string Description =>
        "Get citation relationships for a ruling. Returns rulings that cite it (inbound) and/or rulings it cites (outbound). Use for precedent chain analysis.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "rulingId": { "type": "string", "format": "uuid", "description": "UUID of the ruling to get citations for." },
            "direction": { "type": "string", "enum": ["inbound", "outbound", "both"], "description": "Citation direction. Default: both." },
            "depth": { "type": "integer", "description": "Citation chain depth (1-3 hops). Default: 1. Use >1 for multi-hop precedent chain analysis." }
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

        var direction = "both";
        if (root.TryGetProperty("direction", out var dirEl) && dirEl.ValueKind == JsonValueKind.String)
            direction = dirEl.GetString() ?? "both";

        var depth = Math.Clamp(
            root.TryGetProperty("depth", out var depthEl) && depthEl.TryGetInt32(out var d) ? d : 1,
            1, 3);

        var graph = context.Services.GetRequiredService<IGraphService>();
        var rulingRepo = context.Services.GetRequiredService<IRulingRepository>();

        var sb = new StringBuilder();
        sb.AppendLine($"[get_ruling_citations: {rulingId} (depth={depth})]");
        sb.AppendLine();

        if (depth > 1)
        {
            var chain = await graph.GetCitationChainAsync(rulingId, depth, cancellationToken);
            var byDepth = chain.Where(n => n.Depth > 0).GroupBy(n => n.Depth).OrderBy(g => g.Key);

            foreach (var group in byDepth)
            {
                sb.AppendLine($"--- Hop {group.Key} ({group.Count()} rulings) ---");
                foreach (var node in group)
                {
                    sb.AppendLine($"  {node.CaseTitle} | ID: {node.RulingId} | {node.RulingDate:yyyy-MM-dd} | {node.CitationType} | {node.LegalBranch ?? "N/A"}");
                    context.ResolvedRulingIds.Add(node.RulingId);
                }
                sb.AppendLine();
            }

            sb.AppendLine($"Total: {chain.Count - 1} rulings in citation chain (depth {depth}).");
            return sb.ToString();
        }

        var outboundCount = 0;
        var inboundCount = 0;

        if (direction is "outbound" or "both")
        {
            var outbound = await graph.GetOutboundCitationsAsync(rulingId, cancellationToken);
            outboundCount = outbound.Count;

            var resolvedIds = outbound
                .Where(c => c.TargetRulingId.HasValue)
                .Select(c => c.TargetRulingId!.Value)
                .Distinct()
                .ToList();
            var metadata = resolvedIds.Count > 0
                ? await rulingRepo.GetChatMetadataBatchAsync(resolvedIds, cancellationToken)
                : new Dictionary<Guid, RulingChatMetadata>();

            sb.AppendLine($"Outbound Citations (this ruling cites): {outbound.Count}");
            for (var i = 0; i < outbound.Count; i++)
            {
                var c = outbound[i];
                if (c.TargetRulingId.HasValue && metadata.TryGetValue(c.TargetRulingId.Value, out var m))
                    sb.AppendLine($"{i + 1}. {c.ExternalAlias} → Case: {m.CaseTitle} | ID: {c.TargetRulingId} | Type: {c.CitationType}");
                else
                    sb.AppendLine($"{i + 1}. {c.ExternalAlias} → (unresolved) | Type: {c.CitationType}");
            }
            sb.AppendLine();
        }

        if (direction is "inbound" or "both")
        {
            var inbound = await graph.GetInboundCitationsAsync(rulingId, cancellationToken);
            inboundCount = inbound.Count;

            var sourceIds = inbound.Select(c => c.SourceRulingId).Distinct().ToList();
            var metadata = sourceIds.Count > 0
                ? await rulingRepo.GetChatMetadataBatchAsync(sourceIds, cancellationToken)
                : new Dictionary<Guid, RulingChatMetadata>();

            sb.AppendLine($"Inbound Citations (cited by): {inbound.Count}");
            for (var i = 0; i < inbound.Count; i++)
            {
                var c = inbound[i];
                if (metadata.TryGetValue(c.SourceRulingId, out var m))
                    sb.AppendLine($"{i + 1}. Case: {m.CaseTitle} | ID: {c.SourceRulingId} | Date: {m.RulingDate:yyyy-MM-dd} | Type: {c.CitationType}");
                else
                    sb.AppendLine($"{i + 1}. ID: {c.SourceRulingId} | Type: {c.CitationType}");
            }
            sb.AppendLine();
        }

        sb.AppendLine($"Total: {outboundCount} outbound, {inboundCount} inbound.");
        return sb.ToString();
    }
}
