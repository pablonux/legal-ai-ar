using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Admin.Jobs.DTOs;

public record StuckPipelineDocumentDto(
    Guid Id,
    string ExternalId,
    string CurrentStage,
    string Status);

public record WorkerAuditDto(
    string WorkerType,
    bool IsPaused,
    DateTime? PauseUpdatedAtUtc,
    int SignalRConnectedInstances);

public record DocumentStatusCountsDto(
    int Pending,
    int Processing,
    int Completed,
    int Failed,
    int Discarded,
    int Cancelled,
    int Total,
    int OutstandingPendingOrProcessing);

/// <summary>
/// Heuristic assessment for when operators may apply DB-only fixes (counters, orphan Pending tail) with lower risk.
/// </summary>
public record StructuralRepairSafetyDto(
    bool NoProcessingRowsForJob,
    int ApproxMainPipelineQueueMessageCount,
    bool AnyWorkerConnectedSignalR,
    /// <summary>
    /// Heuristic: no <c>Processing</c> rows for this job and all main pipeline queues report ~0 messages.
    /// Does not prove absence of messages for this job (queues are not keyed by job) nor in-flight dequeue visibility.
    /// </summary>
    bool SuggestedAdministrativeSafeWindow,
    IReadOnlyList<string> Caveats);

public record JobAuditDto(
    Guid JobId,
    string SourceName,
    string JobStatus,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int JobDocumentsDiscovered,
    int JobDocumentsSkipped,
    int JobDocumentsCrawled,
    int JobDocumentsParsed,
    int JobDocumentsEnriched,
    int JobDocumentsPersisted,
    int JobDocumentsIndexed,
    int JobDocumentsFailed,
    int DocumentsTotalRows,
    DocumentStatusCountsDto DbCountsByStatus,
    int IndexedMinusCompletedDelta,
    int FailedMinusFailedRowsDelta,
    bool SatisfiesCompletionFormula,
    bool FormulaSatisfiedButStillOutstanding,
    IReadOnlyList<WorkerAuditDto> Workers,
    IReadOnlyList<StuckPipelineDocumentDto> RiskDocuments,
    StructuralRepairSafetyDto StructuralRepairSafety,
    bool InfrastructureDegraded,
    DateTime? DegradedSinceUtc,
    string? DegradedReason,
    IReadOnlyList<string> Notes,
    DateTime AuditedAtUtc);

public record RepairJobAuditTailResult(int AffectedCount, string Message);

public record ReconcileJobCountersResultDto(
    IngestionPipelineCounters Previous,
    IngestionPipelineCounters Updated,
    bool JobCompletionApplied);
