using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Master/reference entity for a court/tribunal.
/// Treated as nomenclated data with stable identifiers per source.
/// </summary>
public class Court
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ExternalCode { get; set; }
    public string JurisdictionArea { get; set; } = string.Empty;
    public string Territory { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public bool IsVerified { get; set; }

    public CourtType? CourtCategory { get; set; }
    public Fuero? Fuero { get; set; }
    public int? InstanceLevel { get; set; }
    public GovernmentLevel? GovernmentLevel { get; set; }
    public int? ParentCourtId { get; set; }

    public Court? ParentCourt { get; set; }
    public ICollection<Court> ChildCourts { get; set; } = new List<Court>();
    public ICollection<Ruling> Rulings { get; set; } = new List<Ruling>();
    public ICollection<JudicialOffice> JudicialOffices { get; set; } = new List<JudicialOffice>();
}
