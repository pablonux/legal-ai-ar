namespace LegalAiAr.Core.Entities;

/// <summary>
/// Join entity: Sumario to Keyword (vocesSumario from CSJN API).
/// </summary>
public class SumarioKeyword
{
    public int SumarioId { get; set; }
    public int KeywordId { get; set; }
    public int SortOrder { get; set; }

    public Sumario Sumario { get; set; } = null!;
    public Keyword Keyword { get; set; } = null!;
}
