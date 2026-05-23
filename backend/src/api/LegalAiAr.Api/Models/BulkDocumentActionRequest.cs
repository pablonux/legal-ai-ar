using System.Text.Json.Serialization;
using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for POST /api/admin/jobs/{id}/documents/action.
/// </summary>
public class BulkDocumentActionRequest
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PipelineStage Stage { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BulkDocumentAction Action { get; set; }
}
