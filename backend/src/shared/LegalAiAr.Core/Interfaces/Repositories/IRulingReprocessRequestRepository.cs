using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IRulingReprocessRequestRepository
{
    Task<RulingReprocessRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RulingReprocessRequest?> GetActiveByRulingIdAsync(Guid rulingId, CancellationToken cancellationToken = default);

    Task<RulingReprocessRequest?> GetActiveByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RulingReprocessRequest>> ListAsync(
        RulingReprocessRequestStatus? statusFilter,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(RulingReprocessRequestStatus? statusFilter, CancellationToken cancellationToken = default);

    Task AddAsync(RulingReprocessRequest request, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
