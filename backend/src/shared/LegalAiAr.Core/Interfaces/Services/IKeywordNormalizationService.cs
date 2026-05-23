using LegalAiAr.Core.Entities;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Normalizes keywords by linking them to SAIJ thesaurus descriptors.
/// </summary>
public interface IKeywordNormalizationService
{
    /// <summary>
    /// Attempts to match a keyword description to a thesaurus term.
    /// Returns the ThesaurusTerm.Id if matched, null otherwise.
    /// </summary>
    Task<int?> ResolveAsync(string keywordDescription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Normalizes all unlinked keywords in the database.
    /// Returns (matched, total) counts.
    /// </summary>
    Task<(int Matched, int Total)> NormalizeAllAsync(Action<string>? onProgress = null, CancellationToken cancellationToken = default);
}
