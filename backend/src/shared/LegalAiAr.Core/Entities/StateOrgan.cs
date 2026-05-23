using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

public class StateOrgan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Abbreviation { get; set; }
    public OrganBranch Branch { get; set; }
    public GovernmentLevel? GovernmentLevel { get; set; }
    public string? JurisdictionArea { get; set; }
    public int? ParentOrganId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StateOrgan? ParentOrgan { get; set; }
    public ICollection<StateOrgan> ChildOrgans { get; set; } = new List<StateOrgan>();
    public ICollection<Statute> IssuedStatutes { get; set; } = new List<Statute>();
}
