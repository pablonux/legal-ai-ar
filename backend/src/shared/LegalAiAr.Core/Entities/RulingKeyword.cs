namespace LegalAiAr.Core.Entities;

/// <summary>
/// Many-to-many: Ruling to Keyword with sort order.
/// </summary>
public class RulingKeyword
{
    public Guid RulingId { get; set; }
    public int KeywordId { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ruling Ruling { get; set; } = null!;
    public Keyword Keyword { get; set; } = null!;
}
