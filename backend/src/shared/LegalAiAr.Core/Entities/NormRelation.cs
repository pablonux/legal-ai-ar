using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Directed relationship between two legal norms (derogates, amends, regulates, complements).
/// </summary>
public class NormRelation
{
    public int Id { get; set; }
    public int SourceStatuteId { get; set; }
    public int TargetStatuteId { get; set; }
    public NormRelationType RelationType { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Statute SourceStatute { get; set; } = null!;
    public Statute TargetStatute { get; set; } = null!;
}
