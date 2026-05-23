using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// A person intervenes in the production of a ruling in a specific role,
/// optionally as part of a specific vote. Replaces the former RulingJudge entity.
/// </summary>
public class RulingParticipation
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public int PersonId { get; set; }
    public int? VoteId { get; set; }
    public RulingRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ruling Ruling { get; set; } = null!;
    public Person Person { get; set; } = null!;
    public Vote? Vote { get; set; }
}
