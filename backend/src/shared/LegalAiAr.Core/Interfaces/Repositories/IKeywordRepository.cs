using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IKeywordRepository
{
    Task<Keyword?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Keyword> GetOrCreateAsync(int? externalCode, string description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves many keywords in fewer round-trips than repeated <see cref="GetOrCreateAsync"/>.
    /// Duplicate keys in <paramref name="keys"/> are deduplicated; the same <see cref="Keyword"/> instance is shared.
    /// </summary>
    Task<IReadOnlyDictionary<KeywordLookupKey, Keyword>> GetOrCreateBatchAsync(
        IReadOnlyCollection<KeywordLookupKey> keys,
        CancellationToken cancellationToken = default);
}
