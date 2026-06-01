using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Repositories;

/// <summary>
/// Repository for the Documents pipeline tracking table.
/// </summary>
public interface IDocumentRepository
{
    Task<Document> CreateAsync(Document document, CancellationToken cancellationToken = default);

    Task UpdateAsync(Document document, CancellationToken cancellationToken = default);

    Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a document with this (SourceId, ExternalId) already exists.
    /// Used for deduplication at the Discoverer/Fetcher stage.
    /// </summary>
    Task<bool> ExistsByExternalIdAsync(int sourceId, string externalId, CancellationToken cancellationToken = default);

    Task<Document?> GetByExternalIdAsync(int sourceId, string externalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Latest pipeline document row linked to a persisted ruling (by <see cref="Document.RulingId"/>).
    /// </summary>
    Task<Document?> GetLatestByRulingIdAsync(Guid rulingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a document to Fetcher/Pending for full admin reprocess (any prior status/stage).
    /// </summary>
    Task ResetForReprocessAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Advances the document to the next stage with status Pending.
    /// Clears error fields and resets RetryCount.
    /// </summary>
    Task AdvanceStageAsync(Guid id, PipelineStage nextStage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the document as Processing at the expected stage.
    /// Returns false if the document is not at the expected stage (idempotency guard).
    /// </summary>
    Task<bool> SetProcessingAsync(Guid id, PipelineStage expectedStage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the document as Failed at its current stage with error details.
    /// </summary>
    Task SetFailedAsync(Guid id, string errorMessage, string errorType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the document as Completed (final stage reached successfully).
    /// </summary>
    Task SetCompletedAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates BlobPath and ContentHash after fetch.
    /// </summary>
    Task SetFetchResultAsync(Guid id, string blobPath, string contentHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the persisted entity FK after the Persister stage completes.
    /// </summary>
    Task SetEntityIdAsync(Guid id, Guid? rulingId, int? statuteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns documents for a job, optionally filtered by stage and status.
    /// </summary>
    Task<IReadOnlyList<Document>> GetByJobAsync(
        Guid jobId,
        PipelineStage? stageFilter = null,
        DocumentStatus? statusFilter = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Failed documents for a job at the given stage, oldest activity first
    /// (<see cref="Document.LastUpdatedAt"/> fallback <see cref="Document.CreatedAt"/>).
    /// Used for small batched reprocess from admin without touching other failed rows.
    /// </summary>
    Task<IReadOnlyList<Document>> GetFailedDocumentsOldestFirstAsync(
        Guid jobId,
        PipelineStage stage,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts documents for a job using the same filters as <see cref="GetByJobAsync"/>.
    /// </summary>
    Task<int> CountByJobAsync(
        Guid jobId,
        PipelineStage? stageFilter = null,
        DocumentStatus? statusFilter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns aggregated counts per stage for a job.
    /// </summary>
    Task<IReadOnlyDictionary<PipelineStage, StageSummary>> GetStageSummariesAsync(
        Guid jobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk-updates documents matching (jobId, stage, fromStatus) to a new status.
    /// Used for mass reprocess/discard/cancel operations.
    /// </summary>
    Task<int> BulkUpdateStatusAsync(
        Guid jobId,
        PipelineStage stage,
        DocumentStatus fromStatus,
        DocumentStatus toStatus,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Selects up to <paramref name="maxDocuments"/> rows for this job that are
    /// <see cref="DocumentStatus.Failed"/> at <see cref="PipelineStage.Parser"/>, applies optional filters,
    /// moves each to <see cref="PipelineStage.Fetcher"/> with <see cref="DocumentStatus.Pending"/>, clears errors,
    /// and returns the updated entities (for publishing Fetcher queue messages).
    /// </summary>
    Task<IReadOnlyList<Document>> TakeParserFailedRewindToFetcherPendingAsync(
        Guid jobId,
        bool onlyCsjnCacheMiss,
        string? errorMessageContains,
        int? sourceId,
        int maxDocuments,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Transitions a single failed document for a job to Pending (reprocess) or Discarded.
    /// Returns true if exactly one row was updated.
    /// </summary>
    Task<bool> TryTransitionSingleFailedAsync(
        Guid documentId,
        Guid jobId,
        DocumentStatus targetStatus,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the job has any documents still pending or processing.
    /// Used to determine if the job can be marked as completed.
    /// </summary>
    Task<bool> HasPendingDocumentsAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts documents in Pending or Processing per job (batch).
    /// </summary>
    Task<IReadOnlyDictionary<Guid, int>> CountOutstandingDocumentsByJobIdsAsync(
        IReadOnlyCollection<Guid> jobIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts documents for one job grouped by <see cref="DocumentStatus"/> (truth from <c>Documents</c>).
    /// </summary>
    Task<DocumentStatusCountSet> GetDocumentStatusCountsForJobAsync(
        Guid jobId,
        CancellationToken cancellationToken = default);

/// <summary>
/// Candidate rows for operator review: (1) <see cref="DocumentStatus.Pending"/> at Persister or Indexer
/// (often a lost queue message after a dequeue skip), and (2) any <see cref="DocumentStatus.Processing"/> row.
/// </summary>
/// <remarks>
/// <see cref="DocumentStatus.Processing"/> is not automatically an error: under a healthy pipeline, workers
/// legitimately set this while a message is being handled. After outages, some documents can be lost mid-flight
/// while others on the queue continue; those losses may only become visible as stray <c>Processing</c> rows once
/// the rest of the job has finished, queues are empty, workers are idle, and those rows still do not complete.
/// Interpretation always depends on worker connectivity, queue depth, and whether the job is still advancing.
/// </remarks>
    Task<IReadOnlyList<Document>> GetAuditRiskPipelineDocumentsForJobAsync(
        Guid jobId,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Startup recovery: resets documents stuck in Processing status at a specific stage back to Pending.
    /// Returns the number of documents reset.
    /// </summary>
    Task<int> ResetProcessingToPendingAsync(PipelineStage stage, CancellationToken cancellationToken = default);

    /// <summary>
    /// For one job, resets <see cref="DocumentStatus.Processing"/> → <see cref="DocumentStatus.Pending"/> when the row
    /// has not been updated since at least <paramref name="minAgeMinutes"/> (staleness guard vs live workers).
    /// </summary>
    Task<int> ResetStaleProcessingToPendingForJobAsync(
        Guid jobId,
        PipelineStage? stage,
        int minAgeMinutes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets <see cref="Document.FetchPdfTimeoutSeconds"/> for a document in a job. Null clears the override.
    /// </summary>
    Task<bool> TrySetFetchPdfTimeoutAsync(
        Guid jobId,
        Guid documentId,
        int? fetchPdfTimeoutSeconds,
        CancellationToken cancellationToken = default);
}
