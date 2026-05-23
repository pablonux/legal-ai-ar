using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Graph;

/// <summary>
/// Generates Summary, KeyFindings, and refined Title for communities
/// by gathering member entity metadata and calling the LLM (nano model).
/// </summary>
public class CommunitySummaryService : ICommunitySummaryService
{
    private readonly AppDbContext _context;
    private readonly IEnrichmentService _enrichmentService;
    private readonly ILogger<CommunitySummaryService> _logger;

    private const string SchemaName = "community_summary";

    private const string SystemPrompt = """
        Sos un analista juridico argentino. Dada una comunidad de fallos judiciales relacionados entre si, genera: 1) un titulo descriptivo breve (max 100 caracteres), 2) un resumen de 3-5 oraciones que describa el tema comun, los actores principales y la posicion jurisprudencial, y 3) hallazgos clave (2-3 bullet points con los insights mas relevantes). Responde en castellano. Solo JSON.
        """;

    private const string JsonSchema = """
        {
          "type": "object",
          "properties": {
            "title": { "type": "string" },
            "summary": { "type": "string" },
            "keyFindings": { "type": "string" }
          },
          "required": ["title", "summary", "keyFindings"],
          "additionalProperties": false
        }
        """;

    public CommunitySummaryService(
        AppDbContext context,
        IEnrichmentService enrichmentService,
        ILogger<CommunitySummaryService> logger)
    {
        _context = context;
        _enrichmentService = enrichmentService;
        _logger = logger;
    }

    public async Task<int> GenerateSummariesAsync(CancellationToken cancellationToken = default)
    {
        var communities = await _context.GraphCommunities
            .Include(c => c.Memberships)
            .Where(c => c.Summary == "")
            .OrderBy(c => c.Level)
            .ThenByDescending(c => c.EntityCount)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Generating summaries for {Count} communities", communities.Count);
        var summarized = 0;

        foreach (var community in communities)
        {
            try
            {
                var description = await BuildCommunityDescriptionAsync(community, cancellationToken);
                var userPrompt = $"""
                    Comunidad de {community.EntityCount} entidades (Level {community.Level}):
                    Titulo provisional: {community.Title}

                    Composicion:
                    {description}

                    Genera titulo, resumen y hallazgos clave.
                    """;

                var json = await _enrichmentService.GetStructuredOutputAsync(
                    SystemPrompt, userPrompt, SchemaName, JsonSchema, cancellationToken);

                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
                var summary = root.TryGetProperty("summary", out var s) ? s.GetString() : null;
                var keyFindings = root.TryGetProperty("keyFindings", out var k) ? k.GetString() : null;

                if (!string.IsNullOrWhiteSpace(title))
                    community.Title = title.Length > 500 ? title[..497] + "..." : title;
                if (!string.IsNullOrWhiteSpace(summary))
                    community.Summary = summary;
                if (!string.IsNullOrWhiteSpace(keyFindings))
                    community.KeyFindings = keyFindings;

                await _context.SaveChangesAsync(cancellationToken);
                summarized++;

                _logger.LogDebug("Summarized community {Id}: {Title}", community.Id, community.Title);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Failed to summarize community {Id}, skipping", community.Id);
            }
        }

        _logger.LogInformation("Summarized {Summarized}/{Total} communities", summarized, communities.Count);
        return summarized;
    }

    private async Task<string> BuildCommunityDescriptionAsync(
        GraphCommunity community, CancellationToken ct)
    {
        var parts = new List<string>();

        var rulingIds = community.Memberships
            .Where(m => m.EntityType == "Ruling")
            .Select(m => Guid.TryParse(m.EntityId, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .Take(10)
            .ToList();

        if (rulingIds.Count > 0)
        {
            var rulings = await _context.Rulings
                .Where(r => rulingIds.Contains(r.Id))
                .Select(r => new { r.CaseTitle, r.RulingDate, r.LegalBranch, r.Summary })
                .ToListAsync(ct);

            parts.Add($"Fallos ({rulings.Count}):");
            foreach (var r in rulings.Take(5))
            {
                var s = r.Summary != null && r.Summary.Length > 150 ? r.Summary[..150] + "..." : r.Summary;
                parts.Add($"  - {r.CaseTitle} ({r.RulingDate:yyyy-MM-dd}, {r.LegalBranch}): {s}");
            }
            if (rulings.Count > 5)
                parts.Add($"  ... y {rulings.Count - 5} mas");
        }

        var personIds = community.Memberships
            .Where(m => m.EntityType == "Person")
            .Select(m => int.TryParse(m.EntityId, out var i) ? i : 0)
            .Where(i => i > 0)
            .Take(10)
            .ToList();

        if (personIds.Count > 0)
        {
            var persons = await _context.Persons
                .Where(p => personIds.Contains(p.Id))
                .Select(p => p.DisplayName)
                .ToListAsync(ct);
            parts.Add($"Personas: {string.Join(", ", persons)}");
        }

        var statuteIds = community.Memberships
            .Where(m => m.EntityType == "Statute")
            .Select(m => int.TryParse(m.EntityId, out var i) ? i : 0)
            .Where(i => i > 0)
            .Take(10)
            .ToList();

        if (statuteIds.Count > 0)
        {
            var statutes = await _context.Statutes
                .Where(s => statuteIds.Contains(s.Id))
                .Select(s => $"{s.Number} {s.Name}")
                .ToListAsync(ct);
            parts.Add($"Normas: {string.Join(", ", statutes)}");
        }

        return string.Join("\n", parts);
    }
}
