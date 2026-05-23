using System.Text.Json.Serialization;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Api.Models;

/// <summary>
/// POST /api/admin/jobs/{id}/documents/reprocess-next
/// </summary>
public class ReprocessNextFailedRequest
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PipelineStage Stage { get; set; }

    /// <summary>Clamp 1–50 server-side; default 10.</summary>
    public int? Take { get; set; }
}
