namespace LegalAiAr.Api.Models;

/// <summary>Request body for POST /api/admin/jobs/thesaurus.</summary>
public sealed class StartThesaurusIngestJobRequest
{
    /// <summary>When true (default), runs keyword → thesaurus linking for rows without ThesaurusTermId after the crawl.</summary>
    public bool NormalizeKeywords { get; set; } = true;
}
