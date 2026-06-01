using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Admin queue entry for full reprocessing of a single ruling (Fetcher through Indexer).
/// </summary>
public class RulingReprocessRequest
{
    public Guid Id { get; set; }
    public Guid RulingId { get; set; }
    public Guid DocumentId { get; set; }
    public RulingReprocessRequestStatus Status { get; set; } = RulingReprocessRequestStatus.Queued;
    public bool UseCache { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }

    public Ruling Ruling { get; set; } = null!;
    public Document Document { get; set; } = null!;
}
