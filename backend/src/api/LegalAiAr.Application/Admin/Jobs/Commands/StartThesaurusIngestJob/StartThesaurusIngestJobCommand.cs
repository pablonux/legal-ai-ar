using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Commands.StartThesaurusIngestJob;

/// <summary>
/// Starts a background thesaurus crawl (TemaTres API) and optional keyword backfill.
/// Persists an IngestionJob with EntityType Thesaurus.
/// </summary>
public record StartThesaurusIngestJobCommand(bool NormalizeKeywords = true) : IRequest<StartThesaurusIngestJobResult>;

public record StartThesaurusIngestJobResult(bool Success, string Message, Guid JobId);
