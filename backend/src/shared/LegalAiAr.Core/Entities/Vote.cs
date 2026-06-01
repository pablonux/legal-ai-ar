using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// A vote within a collegiate ruling. Groups persons who vote together in the same position.
/// A CSJN ruling typically has 2-4 votes: majority, dissent, concurrence(s).
/// </summary>
public class Vote
{
    public int Id { get; set; }
    public Guid RulingId { get; set; }
    public VoteType VoteType { get; set; }
    public string? Pages { get; set; }
    public string? Summary { get; set; }

    public Ruling Ruling { get; set; } = null!;
    public ICollection<RulingParticipation> Participations { get; set; } = new List<RulingParticipation>();
}
