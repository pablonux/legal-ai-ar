using System.Text;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Thesaurus;

public sealed class ThesaurusContextProvider : IThesaurusContextProvider
{
    private readonly IThesaurusRepository _repo;
    private readonly ILogger<ThesaurusContextProvider> _logger;

    public ThesaurusContextProvider(IThesaurusRepository repo, ILogger<ThesaurusContextProvider> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<string?> GetContextAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return null;

        var terms = await _repo.SearchWithRelationsAsync(query.Trim(), limit: 5, cancellationToken);
        if (terms.Count == 0) return null;

        var sb = new StringBuilder();
        sb.AppendLine("Contexto del Tesauro SAIJ de Derecho Argentino:");

        foreach (var term in terms)
        {
            sb.Append($"- Descriptor: \"{term.Label}\"");
            if (!string.IsNullOrEmpty(term.BranchName))
                sb.Append($" (rama: {term.BranchName})");
            sb.AppendLine();

            var synonyms = term.RelationsAsSource
                .Where(r => r.RelationType == ThesaurusRelationType.UF)
                .Select(r => r.TargetTerm.Label)
                .ToList();
            if (synonyms.Count > 0)
                sb.AppendLine($"  Sinónimos: {string.Join(", ", synonyms)}");

            var broader = term.RelationsAsSource
                .Where(r => r.RelationType == ThesaurusRelationType.BT)
                .Select(r => r.TargetTerm.Label)
                .ToList();
            if (broader.Count > 0)
                sb.AppendLine($"  Término genérico (TG): {string.Join(", ", broader)}");

            var narrower = term.RelationsAsSource
                .Where(r => r.RelationType == ThesaurusRelationType.NT)
                .Select(r => r.TargetTerm.Label)
                .Take(10)
                .ToList();
            if (narrower.Count > 0)
                sb.AppendLine($"  Términos específicos (TE): {string.Join(", ", narrower)}");

            var related = term.RelationsAsSource
                .Where(r => r.RelationType == ThesaurusRelationType.RT)
                .Select(r => r.TargetTerm.Label)
                .ToList();
            if (related.Count > 0)
                sb.AppendLine($"  Términos relacionados (TR): {string.Join(", ", related)}");
        }

        _logger.LogDebug("Thesaurus context for query '{Query}': {TermCount} descriptors found",
            query, terms.Count);

        return sb.ToString();
    }
}
