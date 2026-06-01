namespace LegalAiAr.Application.Admin.RulingReprocess.DTOs;

public record RulingReprocessRequestDto(
    Guid Id,
    Guid RulingId,
    Guid DocumentId,
    string Status,
    bool UseCache,
    string RequestedBy,
    DateTime RequestedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? ErrorMessage,
    int RetryCount,
    string CaseTitle,
    string ExternalId);

public record RulingReprocessListResult(
    IReadOnlyList<RulingReprocessRequestDto> Items,
    int Total);

public record EnqueueRulingReprocessResult(Guid RequestId, string Message);
