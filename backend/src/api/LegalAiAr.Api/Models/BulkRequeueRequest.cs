namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for POST /api/admin/pipeline/bulk-requeue.
/// </summary>
public class BulkRequeueRequest
{
    /// <summary>
    /// Target stage: enrichment or indexer.
    /// </summary>
    public string Stage { get; set; } = "enrichment";

    /// <summary>
    /// When true, only requeue rulings where LegalBranch is null (not yet ontology-classified).
    /// Defaults to true.
    /// </summary>
    public bool OnlyMissingOntology { get; set; } = true;

    /// <summary>
    /// Optional source filter (e.g. only CSJN source).
    /// </summary>
    public int? SourceId { get; set; }

    /// <summary>
    /// Number of rulings per batch (1-500). Defaults to 50.
    /// </summary>
    public int BatchSize { get; set; } = 50;
}
