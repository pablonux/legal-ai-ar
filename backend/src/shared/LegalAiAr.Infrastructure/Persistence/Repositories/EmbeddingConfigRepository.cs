using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class EmbeddingConfigRepository : IEmbeddingConfigRepository
{
    private readonly AppDbContext _context;

    public EmbeddingConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmbeddingConfig?> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EmbeddingConfigs
            .FirstOrDefaultAsync(c => c.IsActive, cancellationToken);
    }

    public async Task<EmbeddingConfig?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.EmbeddingConfigs
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Seeds v1 config if no configs exist, returns the active config.
    /// </summary>
    public async Task<EmbeddingConfig> EnsureSeededAsync(CancellationToken cancellationToken = default)
    {
        var active = await GetActiveAsync(cancellationToken);
        if (active is not null) return active;

        var v1 = new EmbeddingConfig
        {
            Version = "v1-rule-based",
            EmbeddingModel = "text-embedding-3-large",
            EmbeddingDimensions = 3072,
            ContextualizationMethod = "rule-based-prefix",
            ChunkingStrategy = "fixed-512-overlap-50",
            ChunkSize = 512,
            ChunkOverlap = 50,
            IsActive = true,
            Notes = "Initial config: rule-based metadata prefix, text-embedding-3-large 3072d"
        };

        _context.EmbeddingConfigs.Add(v1);
        await _context.SaveChangesAsync(cancellationToken);
        return v1;
    }

    public async Task UpsertEmbeddingStateAsync(
        Guid rulingId,
        int embeddingConfigId,
        int chunkCount,
        CancellationToken cancellationToken = default)
    {
        var state = await _context.RulingEmbeddingStates
            .FirstOrDefaultAsync(s => s.RulingId == rulingId, cancellationToken);

        if (state is null)
        {
            state = new RulingEmbeddingState
            {
                RulingId = rulingId,
                EmbeddingConfigId = embeddingConfigId,
                EmbeddedAt = DateTime.UtcNow,
                ChunkCount = chunkCount,
                NeedsReembedding = false
            };
            _context.RulingEmbeddingStates.Add(state);
        }
        else
        {
            state.EmbeddingConfigId = embeddingConfigId;
            state.EmbeddedAt = DateTime.UtcNow;
            state.ChunkCount = chunkCount;
            state.NeedsReembedding = false;
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            _context.Entry(state).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            throw;
        }
    }
}
