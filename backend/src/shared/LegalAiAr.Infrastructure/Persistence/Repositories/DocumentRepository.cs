using System.Data;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LegalAiAr.Infrastructure.Persistence.Repositories;

internal class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context) => _context = context;

    public async Task<Document> CreateAsync(Document document, CancellationToken cancellationToken = default)
    {
        document.CreatedAt = DateTime.UtcNow;
        _context.Documents.Add(document);
        await _context.SaveChangesAsync(cancellationToken);
        return document;
    }

    public async Task UpdateAsync(Document document, CancellationToken cancellationToken = default)
    {
        document.LastUpdatedAt = DateTime.UtcNow;
        _context.Documents.Update(document);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Documents.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByExternalIdAsync(int sourceId, string externalId, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .AnyAsync(d => d.SourceId == sourceId && d.ExternalId == externalId, cancellationToken);
    }

    public async Task<Document?> GetByExternalIdAsync(int sourceId, string externalId, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(d => d.SourceId == sourceId && d.ExternalId == externalId, cancellationToken);
    }

    public async Task<Document?> GetLatestByRulingIdAsync(Guid rulingId, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .Where(d => d.RulingId == rulingId)
            .OrderByDescending(d => d.LastUpdatedAt ?? d.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task ResetForReprocessAsync(Guid documentId, CancellationToken cancellationToken = default) =>
        _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE Documents
            SET CurrentStage = {0}, Status = {1}, ErrorMessage = NULL, ErrorType = NULL,
                RetryCount = 0, LastUpdatedAt = {2}
            WHERE Id = {3}
            """,
            [
                PipelineStage.Fetcher.ToString(),
                DocumentStatus.Pending.ToString(),
                DateTime.UtcNow,
                documentId
            ],
            cancellationToken);

    public async Task AdvanceStageAsync(Guid id, PipelineStage nextStage, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE Documents
            SET CurrentStage = {0}, Status = {1}, ErrorMessage = NULL, ErrorType = NULL,
                RetryCount = 0, LastUpdatedAt = {2}
            WHERE Id = {3}
            """,
            [nextStage.ToString(), DocumentStatus.Pending.ToString(), DateTime.UtcNow, id],
            cancellationToken);
    }

    public async Task<bool> SetProcessingAsync(Guid id, PipelineStage expectedStage, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE Documents
            SET Status = {0}, LastUpdatedAt = {1}
            WHERE Id = {2} AND CurrentStage = {3} AND Status IN ({4}, {5})
            """,
            [
                DocumentStatus.Processing.ToString(), DateTime.UtcNow,
                id, expectedStage.ToString(),
                DocumentStatus.Pending.ToString(), DocumentStatus.Failed.ToString()
            ],
            cancellationToken);
        return rows > 0;
    }

    public async Task SetFailedAsync(Guid id, string errorMessage, string errorType, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE Documents
            SET Status = {0}, ErrorMessage = {1}, ErrorType = {2},
                RetryCount = RetryCount + 1, LastUpdatedAt = {3}
            WHERE Id = {4}
            """,
            [DocumentStatus.Failed.ToString(), errorMessage, errorType, DateTime.UtcNow, id],
            cancellationToken);
    }

    public async Task SetCompletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE Documents
            SET Status = {0}, LastUpdatedAt = {1}
            WHERE Id = {2}
            """,
            [DocumentStatus.Completed.ToString(), DateTime.UtcNow, id],
            cancellationToken);
    }

    public async Task SetFetchResultAsync(Guid id, string blobPath, string contentHash, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE Documents
            SET BlobPath = {0}, ContentHash = {1}, LastUpdatedAt = {2}
            WHERE Id = {3}
            """,
            [blobPath, contentHash, DateTime.UtcNow, id],
            cancellationToken);
    }

    public async Task SetEntityIdAsync(Guid id, Guid? rulingId, int? statuteId, CancellationToken cancellationToken = default)
    {
        var pRulingId = new SqlParameter("@rulingId", SqlDbType.UniqueIdentifier)
        { Value = rulingId.HasValue ? rulingId.Value : DBNull.Value };
        var pStatuteId = new SqlParameter("@statuteId", SqlDbType.Int)
        { Value = statuteId.HasValue ? statuteId.Value : DBNull.Value };
        var pNow = new SqlParameter("@now", DateTime.UtcNow);
        var pId = new SqlParameter("@id", id);

        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE Documents SET RulingId = @rulingId, StatuteId = @statuteId, LastUpdatedAt = @now WHERE Id = @id",
            [pRulingId, pStatuteId, pNow, pId],
            cancellationToken);
    }

    private IQueryable<Document> DocumentsForJobQuery(
        Guid jobId,
        PipelineStage? stageFilter,
        DocumentStatus? statusFilter)
    {
        var query = _context.Documents.Where(d => d.IngestionJobId == jobId);

        if (stageFilter.HasValue)
            query = query.Where(d => d.CurrentStage == stageFilter.Value);
        if (statusFilter.HasValue)
            query = query.Where(d => d.Status == statusFilter.Value);

        return query;
    }

    public async Task<IReadOnlyList<Document>> GetByJobAsync(
        Guid jobId,
        PipelineStage? stageFilter = null,
        DocumentStatus? statusFilter = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        return await DocumentsForJobQuery(jobId, stageFilter, statusFilter)
            .OrderByDescending(d => d.LastUpdatedAt ?? d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetFailedDocumentsOldestFirstAsync(
        Guid jobId,
        PipelineStage stage,
        int take,
        CancellationToken cancellationToken = default)
    {
        if (take <= 0)
            return [];

        return await DocumentsForJobQuery(jobId, stage, DocumentStatus.Failed)
            .OrderBy(d => d.LastUpdatedAt ?? d.CreatedAt)
            .ThenBy(d => d.Id)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByJobAsync(
        Guid jobId,
        PipelineStage? stageFilter = null,
        DocumentStatus? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        return await DocumentsForJobQuery(jobId, stageFilter, statusFilter)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<PipelineStage, StageSummary>> GetStageSummariesAsync(
        Guid jobId,
        CancellationToken cancellationToken = default)
    {
        var raw = await _context.Documents
            .Where(d => d.IngestionJobId == jobId)
            .GroupBy(d => d.CurrentStage)
            .Select(g => new
            {
                Stage = g.Key,
                Pending = g.Count(x => x.Status == DocumentStatus.Pending),
                Processing = g.Count(x => x.Status == DocumentStatus.Processing),
                Completed = g.Count(x => x.Status == DocumentStatus.Completed),
                Failed = g.Count(x => x.Status == DocumentStatus.Failed),
                Discarded = g.Count(x => x.Status == DocumentStatus.Discarded),
                Cancelled = g.Count(x => x.Status == DocumentStatus.Cancelled),
            })
            .ToListAsync(cancellationToken);

        return raw.ToDictionary(
            x => x.Stage,
            x => new StageSummary(x.Pending, x.Processing, x.Completed, x.Failed, x.Discarded, x.Cancelled));
    }

    public async Task<int> BulkUpdateStatusAsync(
        Guid jobId,
        PipelineStage stage,
        DocumentStatus fromStatus,
        DocumentStatus toStatus,
        CancellationToken cancellationToken = default)
    {
        return await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE Documents
            SET Status = {0}, LastUpdatedAt = {1}
            WHERE IngestionJobId = {2} AND CurrentStage = {3} AND Status = {4}
            """,
            [toStatus.ToString(), DateTime.UtcNow, jobId, stage.ToString(), fromStatus.ToString()],
            cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> TakeParserFailedRewindToFetcherPendingAsync(
        Guid jobId,
        bool onlyCsjnCacheMiss,
        string? errorMessageContains,
        int? sourceId,
        int maxDocuments,
        CancellationToken cancellationToken = default)
    {
        if (maxDocuments <= 0)
            return [];

        var query = _context.Documents
            .Where(d => d.IngestionJobId == jobId
                && d.CurrentStage == PipelineStage.Parser
                && d.Status == DocumentStatus.Failed);

        if (onlyCsjnCacheMiss)
        {
            const string needle = "CSJN API cache miss";
            query = query.Where(d => d.SourceId == 1 && d.ErrorMessage != null && d.ErrorMessage.Contains(needle));
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(errorMessageContains))
            {
                var needle = errorMessageContains;
                query = query.Where(d => d.ErrorMessage != null && d.ErrorMessage.Contains(needle));
            }

            if (sourceId.HasValue)
                query = query.Where(d => d.SourceId == sourceId.Value);
        }

        var list = await query
            .OrderBy(d => d.LastUpdatedAt ?? d.CreatedAt)
            .ThenBy(d => d.Id)
            .Take(maxDocuments)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var d in list)
        {
            d.CurrentStage = PipelineStage.Fetcher;
            d.Status = DocumentStatus.Pending;
            d.ErrorMessage = null;
            d.ErrorType = null;
            d.RetryCount = 0;
            d.LastUpdatedAt = now;
        }

        if (list.Count > 0)
            await _context.SaveChangesAsync(cancellationToken);

        return list;
    }

    public async Task<bool> TryTransitionSingleFailedAsync(
        Guid documentId,
        Guid jobId,
        DocumentStatus targetStatus,
        CancellationToken cancellationToken = default)
    {
        if (targetStatus != DocumentStatus.Pending && targetStatus != DocumentStatus.Discarded)
            throw new ArgumentOutOfRangeException(nameof(targetStatus));

        int rows;
        if (targetStatus == DocumentStatus.Pending)
        {
            rows = await _context.Database.ExecuteSqlRawAsync(
                """
                UPDATE Documents
                SET Status = {0}, LastUpdatedAt = {1}, ErrorMessage = NULL, ErrorType = NULL, RetryCount = 0
                WHERE Id = {2} AND IngestionJobId = {3} AND Status = {4}
                """,
                [
                    DocumentStatus.Pending.ToString(), DateTime.UtcNow,
                    documentId, jobId, DocumentStatus.Failed.ToString()
                ],
                cancellationToken);
        }
        else
        {
            rows = await _context.Database.ExecuteSqlRawAsync(
                """
                UPDATE Documents
                SET Status = {0}, LastUpdatedAt = {1}
                WHERE Id = {2} AND IngestionJobId = {3} AND Status = {4}
                """,
                [
                    DocumentStatus.Discarded.ToString(), DateTime.UtcNow,
                    documentId, jobId, DocumentStatus.Failed.ToString()
                ],
                cancellationToken);
        }

        return rows == 1;
    }

    public async Task<bool> HasPendingDocumentsAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .AnyAsync(d => d.IngestionJobId == jobId
                && (d.Status == DocumentStatus.Pending || d.Status == DocumentStatus.Processing),
                cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, int>> CountOutstandingDocumentsByJobIdsAsync(
        IReadOnlyCollection<Guid> jobIds,
        CancellationToken cancellationToken = default)
    {
        if (jobIds.Count == 0)
            return new Dictionary<Guid, int>();

        var ids = jobIds as IList<Guid> ?? jobIds.ToList();
        var rows = await _context.Documents
            .Where(d => ids.Contains(d.IngestionJobId)
                && (d.Status == DocumentStatus.Pending || d.Status == DocumentStatus.Processing))
            .GroupBy(d => d.IngestionJobId)
            .Select(g => new { JobId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(x => x.JobId, x => x.Count);
    }

    public async Task<DocumentStatusCountSet> GetDocumentStatusCountsForJobAsync(
        Guid jobId,
        CancellationToken cancellationToken = default)
    {
        var rows = await _context.Documents
            .Where(d => d.IngestionJobId == jobId)
            .GroupBy(d => d.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var map = rows.ToDictionary(x => x.Status, x => x.Count);
        int C(DocumentStatus s) => map.TryGetValue(s, out var n) ? n : 0;

        return new DocumentStatusCountSet
        {
            Pending = C(DocumentStatus.Pending),
            Processing = C(DocumentStatus.Processing),
            Completed = C(DocumentStatus.Completed),
            Failed = C(DocumentStatus.Failed),
            Discarded = C(DocumentStatus.Discarded),
            Cancelled = C(DocumentStatus.Cancelled),
        };
    }

    /// <inheritdoc cref="IDocumentRepository.GetAuditRiskPipelineDocumentsForJobAsync"/>
    public async Task<IReadOnlyList<Document>> GetAuditRiskPipelineDocumentsForJobAsync(
        Guid jobId,
        int take,
        CancellationToken cancellationToken = default)
    {
        if (take <= 0)
            return [];

        return await _context.Documents
            .Where(d => d.IngestionJobId == jobId
                && (
                    (d.Status == DocumentStatus.Pending
                     && (d.CurrentStage == PipelineStage.Persister || d.CurrentStage == PipelineStage.Indexer))
                    || d.Status == DocumentStatus.Processing))
            .OrderBy(d => d.LastUpdatedAt ?? d.CreatedAt)
            .ThenBy(d => d.Id)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TrySetFetchPdfTimeoutAsync(
        Guid jobId,
        Guid documentId,
        int? fetchPdfTimeoutSeconds,
        CancellationToken cancellationToken = default)
    {
        var doc = await _context.Documents.FirstOrDefaultAsync(
            d => d.Id == documentId && d.IngestionJobId == jobId,
            cancellationToken);
        if (doc is null)
            return false;

        doc.FetchPdfTimeoutSeconds = fetchPdfTimeoutSeconds;
        doc.LastUpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> ResetProcessingToPendingAsync(PipelineStage stage, CancellationToken cancellationToken = default)
    {
        return await _context.Database.ExecuteSqlRawAsync(
            """
            UPDATE Documents
            SET Status = {0}, LastUpdatedAt = {1}
            WHERE CurrentStage = {2} AND Status = {3}
            """,
            [DocumentStatus.Pending.ToString(), DateTime.UtcNow, stage.ToString(), DocumentStatus.Processing.ToString()],
            cancellationToken);
    }

    public async Task<int> ResetStaleProcessingToPendingForJobAsync(
        Guid jobId,
        PipelineStage? stage,
        int minAgeMinutes,
        CancellationToken cancellationToken = default)
    {
        if (minAgeMinutes < 0)
            minAgeMinutes = 0;

        var cutoff = DateTime.UtcNow.AddMinutes(-minAgeMinutes);

        if (stage.HasValue)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE Documents
                SET Status = {DocumentStatus.Pending.ToString()},
                    LastUpdatedAt = {DateTime.UtcNow}
                WHERE IngestionJobId = {jobId}
                  AND Status = {DocumentStatus.Processing.ToString()}
                  AND CurrentStage = {stage.Value.ToString()}
                  AND COALESCE(LastUpdatedAt, CreatedAt) <= {cutoff}
                """,
                cancellationToken);
        }

        return await _context.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE Documents
            SET Status = {DocumentStatus.Pending.ToString()},
                LastUpdatedAt = {DateTime.UtcNow}
            WHERE IngestionJobId = {jobId}
              AND Status = {DocumentStatus.Processing.ToString()}
              AND COALESCE(LastUpdatedAt, CreatedAt) <= {cutoff}
            """,
            cancellationToken);
    }
}
