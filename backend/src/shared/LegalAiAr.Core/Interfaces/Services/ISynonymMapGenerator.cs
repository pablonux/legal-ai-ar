namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Generates synonym map rules from thesaurus data for use in Azure AI Search.
/// </summary>
public interface ISynonymMapGenerator
{
    /// <summary>
    /// Builds Solr-format synonym rules from UF (synonym) relations in the thesaurus.
    /// Each line is a comma-separated equivalence group: "preferred, synonym1, synonym2".
    /// </summary>
    Task<string> GenerateSolrRulesAsync(CancellationToken cancellationToken = default);
}
