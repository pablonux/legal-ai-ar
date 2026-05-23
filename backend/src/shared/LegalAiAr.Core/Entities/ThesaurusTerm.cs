namespace LegalAiAr.Core.Entities;

/// <summary>
/// Descriptor from the SAIJ legal thesaurus (Tesauro SAIJ de Derecho Argentino).
/// Each term may be a preferred descriptor or a non-preferred synonym.
/// </summary>
public class ThesaurusTerm
{
    public int Id { get; set; }

    /// <summary>TemaTres term_id from the SAIJ API.</summary>
    public int ExternalId { get; set; }

    /// <summary>Preferred or non-preferred label text.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>True when the term is an accepted descriptor; false for USE redirects.</summary>
    public bool IsPreferred { get; set; }

    /// <summary>Top-level thematic branch (e.g. "Derecho laboral"). Null for non-preferred terms.</summary>
    public string? BranchName { get; set; }

    /// <summary>Depth in the hierarchical tree (0 = top term).</summary>
    public int Depth { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public ICollection<ThesaurusRelation> RelationsAsSource { get; set; } = new List<ThesaurusRelation>();
    public ICollection<ThesaurusRelation> RelationsAsTarget { get; set; } = new List<ThesaurusRelation>();
}
