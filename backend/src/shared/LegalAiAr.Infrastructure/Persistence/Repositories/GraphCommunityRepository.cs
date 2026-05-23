using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class GraphCommunityRepository : IGraphCommunityRepository
{
    private readonly AppDbContext _context;

    public GraphCommunityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GraphCommunity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.GraphCommunities
            .Include(c => c.Memberships)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GraphCommunity>> GetByLevelAsync(int level, CancellationToken cancellationToken = default)
    {
        return await _context.GraphCommunities
            .Where(c => c.Level == level)
            .OrderByDescending(c => c.EntityCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GraphCommunity>> SearchByKeywordAsync(
        string keyword, int topK = 10, CancellationToken cancellationToken = default)
    {
        return await _context.GraphCommunities
            .Where(c => c.Title.Contains(keyword) || c.Summary.Contains(keyword)
                        || (c.KeyFindings != null && c.KeyFindings.Contains(keyword)))
            .OrderByDescending(c => c.EntityCount)
            .Take(topK)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GraphCommunity>> GetCommunitiesForEntityAsync(
        string entityType, string entityId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunityMemberships
            .Where(m => m.EntityType == entityType && m.EntityId == entityId)
            .Select(m => m.Community)
            .OrderByDescending(c => c.Level)
            .ToListAsync(cancellationToken);
    }

    public async Task AddCommunityAsync(GraphCommunity community, CancellationToken cancellationToken = default)
    {
        _context.GraphCommunities.Add(community);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMembershipsAsync(IEnumerable<CommunityMembership> memberships, CancellationToken cancellationToken = default)
    {
        _context.CommunityMemberships.AddRange(memberships);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearAllAsync(CancellationToken cancellationToken = default)
    {
        _context.CommunityMemberships.RemoveRange(_context.CommunityMemberships);
        _context.GraphCommunities.RemoveRange(_context.GraphCommunities);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetCommunityCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GraphCommunities.CountAsync(cancellationToken);
    }
}
