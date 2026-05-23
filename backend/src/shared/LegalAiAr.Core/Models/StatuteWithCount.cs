using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Models;

public sealed record StatuteWithCount(
    int Id,
    string Number,
    string Name,
    NormType? NormType,
    NormativeLevel? NormativeLevel,
    LegalBranch? LegalBranch,
    string? IssuingBody,
    DateOnly? SanctionDate,
    DateOnly? EffectiveTo,
    StatuteStatus? Status,
    int RulingCount)
{
    public bool IsVigente => Status == StatuteStatus.Vigente
        || (Status == null && (EffectiveTo == null || EffectiveTo > DateOnly.FromDateTime(DateTime.UtcNow)));
}
