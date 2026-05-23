using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Graph;

/// <summary>
/// Detects communities from the KB graph using connected components on citations
/// plus attribute-based clustering (shared persons, statutes, legal branch).
/// Produces Level 0 (leaf clusters) and Level 1 (merged clusters by legal branch).
/// </summary>
public class CommunityDetectionService : ICommunityDetectionService
{
    private readonly AppDbContext _context;
    private readonly IGraphCommunityRepository _communityRepo;
    private readonly IEmbeddingConfigRepository _embeddingConfigRepo;
    private readonly ILogger<CommunityDetectionService> _logger;

    private const int MinClusterSize = 3;

    public CommunityDetectionService(
        AppDbContext context,
        IGraphCommunityRepository communityRepo,
        IEmbeddingConfigRepository embeddingConfigRepo,
        ILogger<CommunityDetectionService> logger)
    {
        _context = context;
        _communityRepo = communityRepo;
        _embeddingConfigRepo = embeddingConfigRepo;
        _logger = logger;
    }

    public async Task<int> DetectCommunitiesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting community detection...");

        await _communityRepo.ClearAllAsync(cancellationToken);

        var activeConfig = await _embeddingConfigRepo.GetActiveAsync(cancellationToken);
        var configId = activeConfig?.Id;

        var citations = await _context.Citations
            .Where(c => c.SourceRulingId != Guid.Empty && c.TargetRulingId != null)
            .Select(c => new { c.SourceRulingId, TargetRulingId = c.TargetRulingId!.Value })
            .ToListAsync(cancellationToken);

        var rulingIds = await _context.Rulings
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        var rulingSet = new HashSet<Guid>(rulingIds);

        // Union-Find for connected components on citation graph
        var parent = new Dictionary<Guid, Guid>();
        foreach (var id in rulingIds) parent[id] = id;

        foreach (var edge in citations)
        {
            if (rulingSet.Contains(edge.SourceRulingId) && rulingSet.Contains(edge.TargetRulingId))
                Union(parent, edge.SourceRulingId, edge.TargetRulingId);
        }

        // Also connect rulings that share 2+ persons (same judge panel)
        var participations = await _context.RulingParticipations
            .Select(p => new { p.RulingId, p.PersonId })
            .ToListAsync(cancellationToken);

        var rulingsByPerson = participations
            .GroupBy(p => p.PersonId)
            .Where(g => g.Count() >= 2)
            .ToDictionary(g => g.Key, g => g.Select(p => p.RulingId).ToList());

        foreach (var group in rulingsByPerson.Values)
        {
            for (var i = 1; i < group.Count; i++)
            {
                if (rulingSet.Contains(group[0]) && rulingSet.Contains(group[i]))
                    Union(parent, group[0], group[i]);
            }
        }

        // Build Level 0 communities from connected components
        var components = new Dictionary<Guid, List<Guid>>();
        foreach (var id in rulingIds)
        {
            var root = Find(parent, id);
            if (!components.ContainsKey(root))
                components[root] = new List<Guid>();
            components[root].Add(id);
        }

        var level0Communities = new List<GraphCommunity>();
        var allMemberships = new List<CommunityMembership>();

        var rulingMetadata = await _context.Rulings
            .Select(r => new { r.Id, r.CaseTitle, r.LegalBranch })
            .ToDictionaryAsync(r => r.Id, cancellationToken);

        var rulingKeywords = await _context.RulingKeywords
            .Include(rk => rk.Keyword)
            .Select(rk => new { rk.RulingId, rk.Keyword.Description })
            .ToListAsync(cancellationToken);
        var keywordsByRuling = rulingKeywords
            .GroupBy(k => k.RulingId)
            .ToDictionary(g => g.Key, g => g.Select(k => k.Description).ToList());

        var rulingPersons = await _context.RulingParticipations
            .Include(rp => rp.Person)
            .Select(rp => new { rp.RulingId, rp.PersonId, rp.Person.DisplayName })
            .ToListAsync(cancellationToken);
        var personsByRuling = rulingPersons
            .GroupBy(p => p.RulingId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rulingStatutes = await _context.RulingStatutes
            .Include(rs => rs.Statute)
            .Select(rs => new { rs.RulingId, StatuteId = rs.Statute.Id, rs.Statute.Number, rs.Statute.Name })
            .ToListAsync(cancellationToken);
        var statutesByRuling = rulingStatutes
            .GroupBy(s => s.RulingId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (_, memberIds) in components.Where(c => c.Value.Count >= MinClusterSize))
        {
            var branchCounts = memberIds
                .Where(id => rulingMetadata.ContainsKey(id) && rulingMetadata[id].LegalBranch != null)
                .GroupBy(id => rulingMetadata[id].LegalBranch!.ToString())
                .OrderByDescending(g => g.Count())
                .ToList();
            var dominantBranch = branchCounts.FirstOrDefault()?.Key ?? "General";

            var topKeywords = memberIds
                .Where(id => keywordsByRuling.ContainsKey(id))
                .SelectMany(id => keywordsByRuling[id])
                .GroupBy(k => k)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var title = topKeywords.Count > 0
                ? $"{dominantBranch} - {string.Join(", ", topKeywords)}"
                : $"{dominantBranch} ({memberIds.Count} fallos)";

            var community = new GraphCommunity
            {
                Level = 0,
                Title = title.Length > 500 ? title[..497] + "..." : title,
                Summary = "",
                EntityCount = memberIds.Count,
                EmbeddingConfigId = configId
            };

            await _communityRepo.AddCommunityAsync(community, cancellationToken);
            level0Communities.Add(community);

            var memberships = new List<CommunityMembership>();
            foreach (var rulingId in memberIds)
            {
                memberships.Add(new CommunityMembership
                {
                    CommunityId = community.Id,
                    EntityType = "Ruling",
                    EntityId = rulingId.ToString(),
                    Relevance = 1.0f
                });
            }

            // Add person members
            var personIds = memberIds
                .Where(id => personsByRuling.ContainsKey(id))
                .SelectMany(id => personsByRuling[id])
                .GroupBy(p => p.PersonId)
                .Select(g => g.First())
                .ToList();
            foreach (var p in personIds)
            {
                memberships.Add(new CommunityMembership
                {
                    CommunityId = community.Id,
                    EntityType = "Person",
                    EntityId = p.PersonId.ToString(),
                    Relevance = 0.8f
                });
            }

            // Add statute members
            var statuteIds = memberIds
                .Where(id => statutesByRuling.ContainsKey(id))
                .SelectMany(id => statutesByRuling[id])
                .GroupBy(s => s.StatuteId)
                .Select(g => g.First())
                .ToList();
            foreach (var s in statuteIds)
            {
                memberships.Add(new CommunityMembership
                {
                    CommunityId = community.Id,
                    EntityType = "Statute",
                    EntityId = s.StatuteId.ToString(),
                    Relevance = 0.6f
                });
            }

            await _communityRepo.AddMembershipsAsync(memberships, cancellationToken);
            allMemberships.AddRange(memberships);
        }

        // Build Level 1 communities by merging Level 0 by dominant legal branch
        var level0ByBranch = level0Communities
            .GroupBy(c => c.Title.Split(" - ")[0].Split(" (")[0].Trim())
            .Where(g => g.Count() >= 2)
            .ToList();

        var level1Count = 0;
        foreach (var branchGroup in level0ByBranch)
        {
            var childIds = branchGroup.Select(c => c.Id).ToList();
            var totalEntities = branchGroup.Sum(c => c.EntityCount);

            var level1 = new GraphCommunity
            {
                Level = 1,
                Title = $"{branchGroup.Key} ({totalEntities} fallos, {childIds.Count} clusters)",
                Summary = "",
                EntityCount = totalEntities,
                EmbeddingConfigId = configId
            };

            await _communityRepo.AddCommunityAsync(level1, cancellationToken);
            level1Count++;

            foreach (var child in branchGroup)
            {
                child.ParentCommunityId = level1.Id;
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        var totalCommunities = level0Communities.Count + level1Count;
        _logger.LogInformation(
            "Community detection complete: {Level0} Level-0, {Level1} Level-1, {Total} total communities, {Memberships} memberships",
            level0Communities.Count, level1Count, totalCommunities, allMemberships.Count);

        return totalCommunities;
    }

    private static Guid Find(Dictionary<Guid, Guid> parent, Guid x)
    {
        while (parent[x] != x)
        {
            parent[x] = parent[parent[x]];
            x = parent[x];
        }
        return x;
    }

    private static void Union(Dictionary<Guid, Guid> parent, Guid a, Guid b)
    {
        var ra = Find(parent, a);
        var rb = Find(parent, b);
        if (ra != rb) parent[ra] = rb;
    }
}
