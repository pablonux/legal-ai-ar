using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Tracks a person's appointment at a specific court over time.
/// Enables analysis of career paths across courts.
/// Replaces the former JudgeTenure entity.
/// </summary>
public class JudicialOffice
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public int CourtId { get; set; }
    public JudicialPosition Position { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? DesignationAuthority { get; set; }

    public Person Person { get; set; } = null!;
    public Court Court { get; set; } = null!;
}
