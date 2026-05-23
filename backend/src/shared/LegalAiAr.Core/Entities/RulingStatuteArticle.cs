namespace LegalAiAr.Core.Entities;

/// <summary>
/// Structured reference to a specific article within a statute cited by a ruling.
/// Enables precise queries like "all rulings applying Art. 14 bis of the CN".
/// </summary>
public class RulingStatuteArticle
{
    public int Id { get; set; }
    public int RulingStatuteId { get; set; }
    public string Article { get; set; } = string.Empty;
    public string? Subsection { get; set; }

    public RulingStatute RulingStatute { get; set; } = null!;
}
