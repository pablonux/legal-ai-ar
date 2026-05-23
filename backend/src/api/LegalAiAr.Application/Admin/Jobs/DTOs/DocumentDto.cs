using LegalAiAr.Core.Enums;

namespace LegalAiAr.Application.Admin.Jobs.DTOs;

public record DocumentDto(
    Guid Id,
    Guid IngestionJobId,
    string EntityType,
    int SourceId,
    string ExternalId,
    string? AnalysisId,
    string CurrentStage,
    string Status,
    string? BlobPath,
    string? ContentHash,
    string? ErrorMessage,
    string? ErrorType,
    int RetryCount,
    int? FetchPdfTimeoutSeconds,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt,
    Guid? RulingId,
    int? StatuteId,
    /// <summary>Machine name from the latest <c>DocumentStageLogs</c> row with an error for this document (pipeline worker host).</summary>
    string? LastErrorWorkerInstanceId);

/// <summary>Admin: optional per-document PDF download timeout for the Fetcher (seconds). Null clears.</summary>
public record SetDocumentFetchPdfTimeoutRequestDto(int? TimeoutSeconds);

public record SetDocumentFetchPdfTimeoutResultDto(string Message);

public record JobDocumentsSummaryDto(
    Guid JobId,
    IReadOnlyDictionary<string, StageSummaryDto> Stages,
    int TotalDocuments,
    int TotalFailed,
    int TotalCompleted);

public record StageSummaryDto(
    int Pending,
    int Processing,
    int Completed,
    int Failed,
    int Discarded,
    int Cancelled)
{
    public int Total => Pending + Processing + Completed + Failed + Discarded + Cancelled;
}
