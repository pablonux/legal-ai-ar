using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Crawlers.Commands.RunCrawler;
using LegalAiAr.Application.Admin.Crawlers.Commands.UpdateCrawlerConfig;
using LegalAiAr.Application.Admin.Crawlers.DTOs;
using LegalAiAr.Application.Admin.Crawlers.Queries.GetCrawlers;
using LegalAiAr.Application.Mediation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers.Admin;

/// <summary>
/// Admin API for crawler management: trigger runs, update config, list status.
/// </summary>
[ApiController]
[Route("api/admin/crawlers")]
[Authorize]
public class CrawlersAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public CrawlersAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List all crawler configurations with status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CrawlerConfigDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CrawlerConfigDto>>> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetCrawlersQuery(SourceId: null);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get configuration and status of a specific crawler.
    /// </summary>
    [HttpGet("{sourceId:int}")]
    [ProducesResponseType(typeof(CrawlerConfigDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CrawlerConfigDto>> GetById(int sourceId, CancellationToken cancellationToken)
    {
        var query = new GetCrawlersQuery(SourceId: sourceId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result[0]);
    }

    /// <summary>
    /// Trigger manual crawl for a source.
    /// </summary>
    [HttpPost("{sourceId:int}/run")]
    [ProducesResponseType(typeof(RunCrawlerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RunCrawlerResult>> Run(
        int sourceId,
        [FromBody] RunCrawlerRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Type))
            return BadRequest(new { error = "Type is required." });

        DateOnly? since = null;
        DateOnly? dateFrom = null;
        DateOnly? dateTo = null;

        if (!string.IsNullOrWhiteSpace(request.Since))
        {
            if (!DateOnly.TryParse(request.Since, out var parsed))
                return BadRequest(new { error = "Since must be a valid date (YYYY-MM-DD)." });
            since = parsed;
        }

        if (request.Type.Trim().Equals("by-range", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(request.DateFrom) || string.IsNullOrWhiteSpace(request.DateTo))
                return BadRequest(new { error = "DateFrom and DateTo are required for by-range crawl." });
            if (!DateOnly.TryParse(request.DateFrom, out var fromParsed))
                return BadRequest(new { error = "DateFrom must be a valid date (YYYY-MM-DD)." });
            if (!DateOnly.TryParse(request.DateTo, out var toParsed))
                return BadRequest(new { error = "DateTo must be a valid date (YYYY-MM-DD)." });
            if (fromParsed > toParsed)
                return BadRequest(new { error = "DateFrom must be less than or equal to DateTo." });
            dateFrom = fromParsed;
            dateTo = toParsed;
        }

        var documentType = string.IsNullOrWhiteSpace(request.DocumentType) ? "ruling" : request.DocumentType.Trim();
        var command = new RunCrawlerCommand(sourceId, documentType, request.Type.Trim(), since, dateFrom, dateTo, request.UseCache, request.Reprocess, request.MaxDocuments, request.SkipDocuments);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update crawler configuration. Phase 1: only isEnabled.
    /// </summary>
    [HttpPatch("{sourceId:int}")]
    [ProducesResponseType(typeof(CrawlerConfigDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CrawlerConfigDto>> Update(
        int sourceId,
        [FromBody] UpdateCrawlerConfigRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return BadRequest(new { error = "Request body is required." });

        var command = new UpdateCrawlerConfigCommand(sourceId, request.IsEnabled);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
