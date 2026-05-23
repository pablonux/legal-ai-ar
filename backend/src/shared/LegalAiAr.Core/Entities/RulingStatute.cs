namespace LegalAiAr.Core.Entities;

/// <summary>
/// Many-to-many: Ruling to Statute with cited articles.
/// </summary>
public class RulingStatute
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public int StatuteId { get; set; }
    public string? Articles { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ruling Ruling { get; set; } = null!;
    public Statute Statute { get; set; } = null!;
    public ICollection<RulingStatuteArticle> StructuredArticles { get; set; } = new List<RulingStatuteArticle>();
}
