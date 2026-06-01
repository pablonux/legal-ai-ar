using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// A person (physical or legal) is a party in a judicial proceeding with a specific procedural role.
/// </summary>
public class ProceedingParty
{
    public int Id { get; set; }
    public int JudicialProceedingId { get; set; }
    public int PersonId { get; set; }
    public PartyRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public JudicialProceeding JudicialProceeding { get; set; } = null!;
    public Person Person { get; set; } = null!;
}
