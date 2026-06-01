using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Chat.Tools;

/// <summary>
/// Searches GraphRAG community summaries for broad/global legal questions.
/// Returns community-level summaries that provide high-level jurisprudential context
/// (e.g., "What is the CSJN's position on environmental law?").
/// </summary>
public sealed class SearchCommunitiesTool : IChatTool
{
    public string Name => "search_communities";

    public string Description =>
        "Search knowledge graph communities for broad legal topics. Returns high-level summaries of jurisprudential clusters. " +
        "Use for global/thematic questions like 'What is the court's position on X?' or 'What are the main trends in Y?'. " +
        "Not for specific ruling lookups — use search_rulings for that.";

    public string ParametersSchema => """
        {
          "type": "object",
          "properties": {
            "query": { "type": "string", "description": "Topic or legal concept to search for in community summaries." },
            "level": { "type": "integer", "description": "Community level (0=detailed clusters, 1=broad themes). Default: all levels." }
          },
          "required": ["query"]
        }
        """;

    public async Task<string> ExecuteAsync(
        string argumentsJson, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var root = doc.RootElement;

        var query = root.TryGetProperty("query", out var q) ? q.GetString() ?? "" : "";
        int? level = root.TryGetProperty("level", out var l) && l.ValueKind == JsonValueKind.Number
            ? l.GetInt32()
            : null;

        if (string.IsNullOrWhiteSpace(query))
            return "[search_communities: no query provided]";

        var communityRepo = context.Services.GetRequiredService<IGraphCommunityRepository>();

        var communities = await communityRepo.SearchByKeywordAsync(query, topK: 5, cancellationToken);

        if (level.HasValue)
            communities = communities.Where(c => c.Level == level.Value).ToList();

        if (communities.Count == 0)
            return $"[search_communities: no communities found for '{query}']";

        var sb = new StringBuilder();
        sb.AppendLine($"[search_communities: {communities.Count} communities for '{query}']");
        sb.AppendLine();

        foreach (var c in communities)
        {
            sb.AppendLine($"--- Community #{c.Id} (Level {c.Level}, {c.EntityCount} entities) ---");
            sb.AppendLine($"Title: {c.Title}");
            if (!string.IsNullOrWhiteSpace(c.Summary))
                sb.AppendLine($"Summary: {c.Summary}");
            if (!string.IsNullOrWhiteSpace(c.KeyFindings))
                sb.AppendLine($"Key findings: {c.KeyFindings}");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
