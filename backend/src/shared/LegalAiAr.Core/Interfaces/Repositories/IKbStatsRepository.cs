using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IKbStatsRepository
{
    Task<KbStatsRaw> GetKbStatsAsync(CancellationToken cancellationToken = default);
}
