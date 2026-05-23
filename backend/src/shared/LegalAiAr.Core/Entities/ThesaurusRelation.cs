using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Directed relationship between two thesaurus terms (broader, narrower, synonym, related).
/// </summary>
public class ThesaurusRelation
{
    public int Id { get; set; }
    public int SourceTermId { get; set; }
    public int TargetTermId { get; set; }
    public ThesaurusRelationType RelationType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ThesaurusTerm SourceTerm { get; set; } = null!;
    public ThesaurusTerm TargetTerm { get; set; } = null!;
}
