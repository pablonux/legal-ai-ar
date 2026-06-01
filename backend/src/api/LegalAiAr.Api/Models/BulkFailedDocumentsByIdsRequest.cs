using System.Text.Json.Serialization;
using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;

namespace LegalAiAr.Api.Models;

/// <summary>
/// Body for POST /api/admin/jobs/{jobId}/documents/bulk-by-ids.
/// </summary>
public class BulkFailedDocumentsByIdsRequest
{
    public List<Guid> DocumentIds { get; set; } = [];

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BulkDocumentAction Action { get; set; }
}
