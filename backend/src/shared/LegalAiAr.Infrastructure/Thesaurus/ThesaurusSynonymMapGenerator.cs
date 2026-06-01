using System.Text;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Thesaurus;

/// <summary>
/// Generates Solr-format synonym rules from the thesaurus UF relations.
/// Groups all non-preferred labels under their preferred term into equivalence lines.
/// </summary>
public sealed class ThesaurusSynonymMapGenerator : ISynonymMapGenerator
{
    private readonly IThesaurusRepository _repo;
    private readonly ILogger<ThesaurusSynonymMapGenerator> _logger;

    public ThesaurusSynonymMapGenerator(IThesaurusRepository repo, ILogger<ThesaurusSynonymMapGenerator> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<string> GenerateSolrRulesAsync(CancellationToken cancellationToken = default)
    {
        var pairs = await _repo.GetAllSynonymPairsAsync(cancellationToken);

        // Group by preferred label → list of non-preferred labels
        var groups = pairs
            .GroupBy(p => p.Preferred, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Any())
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

        var sb = new StringBuilder();
        var ruleCount = 0;

        foreach (var group in groups)
        {
            var preferred = NormalizeTerm(group.Key);
            var synonyms = group
                .Select(p => NormalizeTerm(p.NonPreferred))
                .Where(s => !string.IsNullOrWhiteSpace(s) &&
                            !s.Equals(preferred, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (synonyms.Count == 0) continue;

            // Solr equivalence format: "term1, term2, term3"
            sb.AppendLine($"{preferred}, {string.Join(", ", synonyms)}");
            ruleCount++;
        }

        _logger.LogInformation("Generated {RuleCount} synonym rules from {PairCount} UF pairs", ruleCount, pairs.Count);
        return sb.ToString().TrimEnd();
    }

    private static string NormalizeTerm(string term) =>
        term.Trim().Replace(",", " ").Replace("\n", " ").Replace("\r", "");
}
