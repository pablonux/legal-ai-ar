using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Records the origin of each field value in the KB.
/// When a field is updated, the previous record becomes IsCurrent=false
/// and a new one is created with IsCurrent=true.
/// </summary>
public class FieldProvenance
{
    public long Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? PreviousValue { get; set; }
    public int SourceId { get; set; }
    public string? SourceEndpoint { get; set; }
    public string? SourceField { get; set; }
    public InferenceMethod InferenceMethod { get; set; }
    public string? AiModel { get; set; }
    public string? AiPrompt { get; set; }
    public float? AiConfidence { get; set; }
    public Guid IngestionJobId { get; set; }
    public ChangeType ChangeType { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public bool IsCurrent { get; set; } = true;

    public Source Source { get; set; } = null!;
    public IngestionJob IngestionJob { get; set; } = null!;
}
