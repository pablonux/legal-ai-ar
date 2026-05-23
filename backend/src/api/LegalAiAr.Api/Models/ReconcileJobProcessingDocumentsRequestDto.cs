using LegalAiAr.Core.Enums;

namespace LegalAiAr.Api.Models;

public sealed class ReconcileJobProcessingDocumentsRequestDto
{
    public int MinAgeMinutes { get; set; } = 15;

    public PipelineStage? Stage { get; set; }
}
