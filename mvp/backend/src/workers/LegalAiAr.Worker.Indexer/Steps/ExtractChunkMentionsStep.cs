using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Messages;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Steps;

/// <summary>
/// Extracts entity mentions from each chunk via string matching against the ruling's
/// known persons, statutes, and keywords. Persists ChunkEntityMention records for
/// GraphRAG local search (chunk -> entities -> graph traversal).
/// </summary>
public class ExtractChunkMentionsStep
{
    private readonly AppDbContext _context;
    private readonly ILogger<ExtractChunkMentionsStep> _logger;

    public ExtractChunkMentionsStep(AppDbContext context, ILogger<ExtractChunkMentionsStep> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        Guid rulingId,
        IndexerMessage message,
        IReadOnlyList<ChunkData> chunks,
        CancellationToken cancellationToken = default)
    {
        if (chunks.Count == 0) return;

        var candidates = BuildCandidates(message);
        if (candidates.Count == 0) return;

        var mentions = new List<ChunkEntityMention>();

        foreach (var chunk in chunks)
        {
            if (string.IsNullOrWhiteSpace(chunk.Text)) continue;

            foreach (var candidate in candidates)
            {
                if (chunk.Text.Contains(candidate.SearchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    mentions.Add(new ChunkEntityMention
                    {
                        RulingId = rulingId,
                        ChunkIndex = chunk.Index,
                        EntityType = candidate.EntityType,
                        EntityId = candidate.EntityId,
                        MentionType = candidate.MentionType,
                        Confidence = 1.0f
                    });
                }
            }
        }

        if (mentions.Count > 0)
        {
            _context.ChunkEntityMentions.AddRange(mentions);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Extracted {MentionCount} chunk entity mentions for ruling {RulingId} across {ChunkCount} chunks",
            mentions.Count, rulingId, chunks.Count);
    }

    public static List<MentionCandidate> BuildCandidates(IndexerMessage message)
    {
        var candidates = new List<MentionCandidate>();

        foreach (var p in message.Persons)
        {
            if (!string.IsNullOrWhiteSpace(p.LastName))
                candidates.Add(new MentionCandidate("Person", p.LastName, p.LastName, MentionType.Named));
        }

        foreach (var s in message.Statutes)
        {
            if (!string.IsNullOrWhiteSpace(s.Number))
                candidates.Add(new MentionCandidate("Statute", s.Number, s.Number, MentionType.Applied));
            if (!string.IsNullOrWhiteSpace(s.Name) && s.Name.Length >= 5)
                candidates.Add(new MentionCandidate("Statute", s.Number ?? s.Name, s.Name, MentionType.Applied));
        }

        foreach (var k in message.Keywords)
        {
            if (!string.IsNullOrWhiteSpace(k.Description) && k.Description.Length >= 5)
                candidates.Add(new MentionCandidate("Keyword", k.ExternalCode?.ToString() ?? k.Description, k.Description, MentionType.Discussed));
        }

        return candidates;
    }

    public record MentionCandidate(string EntityType, string EntityId, string SearchTerm, MentionType MentionType);
}
