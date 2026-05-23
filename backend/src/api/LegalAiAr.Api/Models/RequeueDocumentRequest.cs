using System.Text.Json;

namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for POST /api/admin/pipeline/requeue-document.
/// </summary>
public class RequeueDocumentRequest
{
    /// <summary>
    /// Target stage: parser, enrichment, or indexer.
    /// </summary>
    public string Stage { get; set; } = string.Empty;

    /// <summary>
    /// Full message payload when client has it (e.g. from DLQ peek).
    /// Mutually exclusive with RulingId.
    /// </summary>
    public JsonElement? Message { get; set; }

    /// <summary>
    /// Ruling ID for backend reconstruction. Mutually exclusive with Message.
    /// </summary>
    public Guid? RulingId { get; set; }
}
