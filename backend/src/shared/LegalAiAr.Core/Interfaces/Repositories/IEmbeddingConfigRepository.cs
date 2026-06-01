using LegalAiAr.Core.Entities;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IEmbeddingConfigRepository
{
    Task<EmbeddingConfig?> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<EmbeddingConfig?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmbeddingConfig> EnsureSeededAsync(CancellationToken cancellationToken = default);
    Task UpsertEmbeddingStateAsync(
        Guid rulingId,
        int embeddingConfigId,
        int chunkCount,
        CancellationToken cancellationToken = default);
}
