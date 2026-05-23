namespace LegalAiAr.Core.Entities;

/// <summary>
/// Official doctrinal extract of a ruling. What lawyers cite.
/// A ruling may have multiple sumarios, each with its own doctrinal text and keywords.
/// Source: getSumariosAnalisis (CSJN).
/// </summary>
public class Sumario
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public int? ExternalId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Volume { get; set; }
    public string? Page { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ruling Ruling { get; set; } = null!;
    public ICollection<SumarioKeyword> SumarioKeywords { get; set; } = new List<SumarioKeyword>();
}
