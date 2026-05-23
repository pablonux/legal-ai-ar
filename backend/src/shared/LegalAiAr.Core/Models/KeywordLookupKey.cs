namespace LegalAiAr.Core.Models;

/// <summary>
/// Identity for batch keyword resolution: CSJN external code when present, otherwise match by description.
/// Precedence matches <c>IKeywordRepository.GetOrCreateAsync</c>.
/// </summary>
public readonly record struct KeywordLookupKey(int? ExternalCode, string Description);
