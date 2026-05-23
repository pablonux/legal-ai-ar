using System.Text.Json.Serialization;
using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;

namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for POST /api/admin/jobs/{jobId}/documents/{documentId}/action.
/// </summary>
public class SingleDocumentActionRequest
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BulkDocumentAction Action { get; set; }
}
