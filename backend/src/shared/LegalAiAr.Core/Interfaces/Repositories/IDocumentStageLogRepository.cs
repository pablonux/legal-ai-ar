using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Interfaces.Repositories;

/// <summary>
/// Repository for pipeline stage timing logs used in benchmarking.
/// </summary>
public interface IDocumentStageLogRepository
{
    Task LogStageAsync(DocumentStageLog log, CancellationToken ct = default);
    Task<IReadOnlyList<DocumentStageLog>> GetByDocumentIdAsync(Guid documentId, CancellationToken ct = default);

    /// <summary>
    /// For each document id, returns the worker instance id from the most recent stage log row that recorded an error (non-empty <see cref="DocumentStageLog.ErrorMessage"/>).
    /// Documents with no such log are omitted from the dictionary.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string?>> GetLastErrorWorkerInstanceByDocumentIdsAsync(
        IReadOnlyCollection<Guid> documentIds,
        CancellationToken ct = default);
    Task<StageMetrics> GetStageMetricsAsync(Guid jobId, PipelineStage stage, CancellationToken ct = default);
    Task<double> GetStageElapsedAsync(Guid jobId, PipelineStage stage, CancellationToken ct = default);
}

/// <summary>
/// Aggregated performance metrics for a pipeline stage within a job.
/// </summary>
public record StageMetrics(
    int Count,
    double AvgDurationMs,
    double P50DurationMs,
    double P95DurationMs,
    double MaxDurationMs,
    double MinDurationMs);
