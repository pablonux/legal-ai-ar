namespace LegalAiAr.Core.Entities;

/// <summary>
/// A lawyer represents a party in a judicial proceeding.
/// Models: "Dr. Garcia (lawyer) represents YPF S.A. (defendant) in case X v. YPF".
/// </summary>
public class LegalRepresentation
{
    public int Id { get; set; }
    public int LawyerPersonId { get; set; }
    public int PartyPersonId { get; set; }
    public int JudicialProceedingId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Person LawyerPerson { get; set; } = null!;
    public Person PartyPerson { get; set; } = null!;
    public JudicialProceeding JudicialProceeding { get; set; } = null!;
}
