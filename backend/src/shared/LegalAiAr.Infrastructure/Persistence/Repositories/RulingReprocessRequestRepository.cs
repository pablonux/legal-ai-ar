using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal sealed class RulingReprocessRequestRepository : IRulingReprocessRequestRepository
{
    private static readonly RulingReprocessRequestStatus[] ActiveStatuses =
    [
        RulingReprocessRequestStatus.Queued,
        RulingReprocessRequestStatus.Running
    ];

    private readonly AppDbContext _context;

    public RulingReprocessRequestRepository(AppDbContext context) => _context = context;

    public Task<RulingReprocessRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.RulingReprocessRequests
            .Include(r => r.Ruling)
            .Include(r => r.Document)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<RulingReprocessRequest?> GetActiveByRulingIdAsync(Guid rulingId, CancellationToken cancellationToken = default) =>
        _context.RulingReprocessRequests
            .FirstOrDefaultAsync(
                r => r.RulingId == rulingId && ActiveStatuses.Contains(r.Status),
                cancellationToken);

    public Task<RulingReprocessRequest?> GetActiveByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default) =>
        _context.RulingReprocessRequests
            .FirstOrDefaultAsync(
                r => r.DocumentId == documentId && ActiveStatuses.Contains(r.Status),
                cancellationToken);

    public async Task<IReadOnlyList<RulingReprocessRequest>> ListAsync(
        RulingReprocessRequestStatus? statusFilter,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = _context.RulingReprocessRequests
            .Include(r => r.Ruling)
            .AsNoTracking()
            .OrderByDescending(r => r.RequestedAt)
            .AsQueryable();

        if (statusFilter.HasValue)
            query = query.Where(r => r.Status == statusFilter.Value);

        return await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(RulingReprocessRequestStatus? statusFilter, CancellationToken cancellationToken = default)
    {
        var query = _context.RulingReprocessRequests.AsQueryable();
        if (statusFilter.HasValue)
            query = query.Where(r => r.Status == statusFilter.Value);
        return query.CountAsync(cancellationToken);
    }

    public async Task AddAsync(RulingReprocessRequest request, CancellationToken cancellationToken = default)
    {
        _context.RulingReprocessRequests.Add(request);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
