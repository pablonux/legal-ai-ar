using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Records each ingestion execution. Enables tracing which job originated each document.
/// Scoped by (EntityType, SourceId) to prevent overlapping jobs.
/// </summary>
public class IngestionJob
{
    public Guid Id { get; set; }
    public int SourceId { get; set; }
    public EntityType EntityType { get; set; } = EntityType.Ruling;
    public string Type { get; set; } = "incremental";
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public string TriggeredBy { get; set; } = "admin";
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "running";

    /// <summary>Total results reported by the source site search (e.g. "Fallos: 450"). Null when unknown.</summary>
    public int? TotalSearchResults { get; set; }
    public int DocumentsDiscovered { get; set; }
    /// <summary>Documents actually crawled (PDF downloaded, uploaded to blob, published to parser).</summary>
    public int DocumentsCrawled { get; set; }
    public int DocumentsParsed { get; set; }
    public int DocumentsEnriched { get; set; }
    public int DocumentsPersisted { get; set; }
    public int DocumentsIndexed { get; set; }
    public int DocumentsSkipped { get; set; }
    public int DocumentsFailed { get; set; }
    public string? ErrorSummary { get; set; }

    /// <summary>
    /// True after at least one discovery batch was successfully enqueued to the Fetcher queue for this job.
    /// </summary>
    public bool DiscoveryBatchPublished { get; set; } = true;

    /// <summary>
    /// Set when a worker reports storage or network degradation for this job (API hub / operator path).
    /// </summary>
    public bool InfrastructureDegraded { get; set; }

    public DateTime? DegradedSinceUtc { get; set; }

    public string? DegradedReason { get; set; }

    /// <summary>Creation traceability: "api" when created via API, or "split:{parentJobId}" when created by auto-split.</summary>
    public string? CreationLog { get; set; }
    /// <summary>FK to parent job when this is a sub-job created by auto-split. Null for original jobs.</summary>
    public Guid? ParentJobId { get; set; }
    /// <summary>1-based index of this partition within the split. Null if not a sub-job.</summary>
    public int? PartitionIndex { get; set; }
    /// <summary>Total number of partitions in the split. Null if not a sub-job.</summary>
    public int? PartitionTotal { get; set; }

    public Source Source { get; set; } = null!;
    public IngestionJob? ParentJob { get; set; }
}
