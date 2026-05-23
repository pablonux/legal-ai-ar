namespace LegalAiAr.Core.Entities;

/// <summary>
/// Thematic keyword for rulings. ExternalCode is CSJN codigoValor; null for other sources.
/// ThesaurusTermId links to the normalized SAIJ thesaurus descriptor when matched.
/// </summary>
public class Keyword
{
    public int Id { get; set; }
    public int? ExternalCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? ThesaurusTermId { get; set; }

    public ThesaurusTerm? ThesaurusTerm { get; set; }
    public ICollection<RulingKeyword> RulingKeywords { get; set; } = new List<RulingKeyword>();
    public ICollection<SumarioKeyword> SumarioKeywords { get; set; } = new List<SumarioKeyword>();
}
