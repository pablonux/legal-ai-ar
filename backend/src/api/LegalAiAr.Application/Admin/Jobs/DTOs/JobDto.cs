namespace LegalAiAr.Application.Admin.Jobs.DTOs;

/// <summary>
/// Job info for admin. Phase 1: synthetic from CrawlerConfigs.
/// </summary>
public record JobDto(
    string Id,
    int SourceId,
    string SourceName,
    string DocumentType,
    string Type,
    string TriggeredBy,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string Status,
    int? TotalSearchResults,
    int DocumentsDiscovered,
    int DocumentsCrawled,
    int DocumentsParsed,
    int DocumentsEnriched,
    int DocumentsPersisted,
    int DocumentsIndexed,
    int DocumentsSkipped,
    int DocumentsFailed,
    /// <summary>Documents still Pending or Processing for this job (ground truth from Documents table).</summary>
    int OutstandingDocuments,
    string? ErrorSummary = null,
    string? CreationLog = null,
    string? DateFrom = null,
    string? DateTo = null,
    string? ParentJobId = null,
    int? PartitionIndex = null,
    int? PartitionTotal = null,
    bool InfrastructureDegraded = false,
    DateTime? DegradedSinceUtc = null,
    string? DegradedReason = null,
    bool DiscoveryBatchPublished = true);
