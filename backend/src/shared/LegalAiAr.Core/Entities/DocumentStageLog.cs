using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Tracks per-document per-stage processing times for pipeline benchmarking.
/// </summary>
public class DocumentStageLog
{
    public long Id { get; set; }
    public Guid DocumentId { get; set; }
    public PipelineStage Stage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public int DurationMs { get; set; }
    public string? WorkerInstanceId { get; set; }
    public string? ErrorMessage { get; set; }

    public Document Document { get; set; } = null!;
}
