using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Citation from one ruling to another. TargetRulingId is null if cited ruling not yet indexed.
/// </summary>
public class Citation
{
    public int Id { get; set; }
    public Guid SourceRulingId { get; set; }
    public Guid? TargetRulingId { get; set; }
    public int? CitedStatuteId { get; set; }
    public string ExternalAlias { get; set; } = string.Empty; // e.g. "Fallos: 328:1883"
    public int? CsjnSummaryId { get; set; }
    public int? CsjnFalloId { get; set; }
    public string? CitationText { get; set; }
    public CitationType CitationType { get; set; } = CitationType.CITES;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ruling SourceRuling { get; set; } = null!;
    public Ruling? TargetRuling { get; set; }
    public Statute? CitedStatute { get; set; }
}
