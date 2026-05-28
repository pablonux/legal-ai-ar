using LegalAiAr.Application.Admin.Jobs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Commands.RewindParserFailedToFetcher;

/// <summary>
/// Moves documents from Parser/Failed back to Fetcher/Pending and publishes Fetcher messages so the
/// Fetcher can re-download (if needed) and re-prime CSJN sjconsulta JSON blobs before Parser runs again.
/// </summary>
/// <param name="JobId">Ingestion job id.</param>
/// <param name="OnlyCsjnCacheMiss">
/// When true (default), only rows with SourceId=1 and ErrorMessage containing "CSJN API cache miss".
/// </param>
/// <param name="ErrorMessageContains">
/// When <paramref name="OnlyCsjnCacheMiss"/> is false, optional substring filter on <c>ErrorMessage</c>.
/// </param>
/// <param name="SourceId">When <paramref name="OnlyCsjnCacheMiss"/> is false, optional source filter.</param>
/// <param name="MaxDocuments">Cap on rows processed in one call (1–20000).</param>
public record RewindParserFailedToFetcherCommand(
    Guid JobId,
    bool OnlyCsjnCacheMiss = true,
    string? ErrorMessageContains = null,
    int? SourceId = null,
    int MaxDocuments = 5000) : IRequest<RewindParserFailedToFetcherResultDto>;

public record RewindParserFailedToFetcherResultDto(
    int DocumentsRewound,
    int MessagesPublished,
    string Message);
