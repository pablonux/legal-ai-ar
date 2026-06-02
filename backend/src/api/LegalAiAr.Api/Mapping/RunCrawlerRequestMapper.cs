using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Crawlers.Commands.RunCrawler;

namespace LegalAiAr.Api.Mapping;

public static class RunCrawlerRequestMapper
{
    public static (RunCrawlerCommand? Command, IResult? Error) TryMap(int sourceId, RunCrawlerRequest? request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Type))
            return (null, Results.BadRequest(new { error = "Type is required." }));

        DateOnly? since = null;
        DateOnly? dateFrom = null;
        DateOnly? dateTo = null;

        if (!string.IsNullOrWhiteSpace(request.Since))
        {
            if (!DateOnly.TryParse(request.Since, out var parsed))
                return (null, Results.BadRequest(new { error = "Since must be a valid date (YYYY-MM-DD)." }));
            since = parsed;
        }

        if (request.Type.Trim().Equals("by-range", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(request.DateFrom) || string.IsNullOrWhiteSpace(request.DateTo))
                return (null, Results.BadRequest(new { error = "DateFrom and DateTo are required for by-range crawl." }));
            if (!DateOnly.TryParse(request.DateFrom, out var fromParsed))
                return (null, Results.BadRequest(new { error = "DateFrom must be a valid date (YYYY-MM-DD)." }));
            if (!DateOnly.TryParse(request.DateTo, out var toParsed))
                return (null, Results.BadRequest(new { error = "DateTo must be a valid date (YYYY-MM-DD)." }));
            if (fromParsed > toParsed)
                return (null, Results.BadRequest(new { error = "DateFrom must be less than or equal to DateTo." }));
            dateFrom = fromParsed;
            dateTo = toParsed;
        }

        var documentType = string.IsNullOrWhiteSpace(request.DocumentType) ? "ruling" : request.DocumentType.Trim();
        var command = new RunCrawlerCommand(
            sourceId,
            documentType,
            request.Type.Trim(),
            since,
            dateFrom,
            dateTo,
            request.UseCache,
            request.Reprocess,
            request.MaxDocuments,
            request.SkipDocuments);

        return (command, null);
    }
}
