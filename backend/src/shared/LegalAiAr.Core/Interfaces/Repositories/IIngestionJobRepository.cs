using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Repositories;

/// <summary>
/// Repository for IngestionJobs. Scoped by (EntityType, SourceId).
/// </summary>
public interface IIngestionJobRepository
{
    Task<IngestionJob> AddAsync(IngestionJob job, CancellationToken cancellationToken = default);
    Task<IngestionJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IngestionJob>> GetAllAsync(int limit = 100, CancellationToken cancellationToken = default);
    Task IncrementDocumentsDiscoveredAsync(Guid jobId, int count, CancellationToken cancellationToken = default);
    Task IncrementDocumentsCrawledAsync(Guid jobId, int count = 1, CancellationToken cancellationToken = default);
    Task IncrementDocumentsParsedAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task IncrementDocumentsEnrichedAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task IncrementDocumentsPersistedAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task IncrementDocumentsIndexedAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task IncrementDocumentsFailedAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrements <see cref="IngestionJob.DocumentsFailed"/> when failed pipeline documents are cleared
    /// (reprocess → Pending, Discard, etc.). Clamped at zero.
    /// </summary>
    Task DecrementDocumentsFailedAsync(Guid jobId, int amount, CancellationToken cancellationToken = default);

    Task SetDocumentsSkippedAsync(Guid jobId, int count, CancellationToken cancellationToken = default);
    Task SetTotalSearchResultsAsync(Guid jobId, int total, CancellationToken cancellationToken = default);
    Task ResetDocumentsDiscoveredAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task UpdateCompletionAsync(Guid jobId, int documentsDiscovered, string status, string? errorSummary, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid jobId, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates whether a discovery batch was published to the Fetcher queue (raw SQL, no EF tracking).
    /// </summary>
    Task SetDiscoveryBatchPublishedAsync(Guid jobId, bool published, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks infrastructure as degraded for the job (raw SQL).
    /// </summary>
    Task SetInfrastructureDegradedAsync(
        Guid jobId,
        bool degraded,
        DateTime? degradedSinceUtc,
        string? reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// If the job was marked finished (<c>success</c>, <c>completed</c>, or <c>partial</c>), move it back to <c>processing</c>
    /// and clear <see cref="IngestionJob.CompletedAt"/> so UI and overlap checks reflect open pipeline work.
    /// </summary>
    Task ResumeProcessingIfTerminalAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if there is at least one IngestionJob for the given (entityType, sourceId)
    /// with status running, pending, or partial. Prevents overlapping jobs.
    /// </summary>
    Task<bool> HasActiveJobAsync(EntityType entityType, int sourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if there is an active job for the same (entityType, sourceId) other than <paramref name="excludeJobId"/>.
    /// Used when resuming discovery for an existing job without treating it as an overlap.
    /// </summary>
    Task<bool> HasActiveJobOtherThanAsync(
        EntityType entityType,
        int sourceId,
        Guid excludeJobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets jobs filtered by entity type and/or source.
    /// </summary>
    Task<IReadOnlyList<IngestionJob>> GetByEntityAndSourceAsync(
        EntityType? entityType = null,
        int? sourceId = null,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels all running/pending jobs for a given (entityType, sourceId).
    /// </summary>
    Task<int> CancelActiveJobsAsync(EntityType entityType, int sourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// If status is 'processing' and (indexed + failed) >= discovered, transitions status to 'success' and sets CompletedAt.
    /// Returns true if the job was completed.
    /// </summary>
    Task<bool> TryCompleteIfDoneAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Overwrites pipeline counters on the job from aggregated <c>Documents</c> rows (same semantics as workers:
    /// each counter counts documents that <b>successfully finished</b> that stage, <see cref="IngestionJob.DocumentsFailed"/>
    /// is the number of rows in <see cref="Enums.DocumentStatus.Failed"/>).
    /// Does not change <see cref="IngestionJob.DocumentsDiscovered"/> or <see cref="IngestionJob.DocumentsSkipped"/>.
    /// </summary>
    /// <returns>Fresh counters read after the update (no-tracking), or null if the job id does not exist.</returns>
    Task<IngestionPipelineCounters?> ReconcilePipelineCountersFromDocumentsAsync(
        Guid jobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a non-pipeline job (e.g. thesaurus full ingest) as finished and sets synthetic stage counters for the admin UI.
    /// </summary>
    Task FinalizeThesaurusIngestJobAsync(
        Guid jobId,
        bool success,
        string? errorSummary,
        CancellationToken cancellationToken = default);
}
