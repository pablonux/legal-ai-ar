using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.Commands;
using LegalAiAr.Application.Proceedings.DTOs;
using LegalAiAr.Application.Proceedings.Models;
using LegalAiAr.Application.Proceedings.Queries;
using LegalAiAr.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProceedingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProceedingsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lists proceedings with optional search and filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProceedingPageDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProceedingPageDto>> List(
        [FromQuery(Name = "q")] string? search = null,
        [FromQuery] ProcessType? processType = null,
        [FromQuery] LegalBranch? legalBranch = null,
        [FromQuery] int? courtId = null,
        [FromQuery] ProcessStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetProceedingsQuery(search, processType, legalBranch, courtId, status, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets proceeding detail with rulings, parties, and representations.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProceedingDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProceedingDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProceedingByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Gets the judicial proceeding chain for a ruling (all rulings in the same case).
    /// </summary>
    [HttpGet("by-ruling/{rulingId:guid}")]
    [ProducesResponseType(typeof(ProceedingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProceedingResponse>> GetByRuling(Guid rulingId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProceedingByRulingQuery(rulingId), cancellationToken);
        return result is not null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Gets the appeal chain for a proceeding: rulings ordered chronologically
    /// with their procedural remedies linking instances.
    /// </summary>
    [HttpGet("{id:int}/appeal-chain")]
    [ProducesResponseType(typeof(AppealChainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppealChainDto>> GetAppealChain(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAppealChainQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Admin: links rulings into proceedings by CaseNumber grouping.
    /// </summary>
    [HttpPost("link")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(LinkProceedingsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<LinkProceedingsResult>> LinkProceedings(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LinkProceedingsCommand(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin: infers ProcessType, ProcessStatus, CourtId, LegalBranch for proceedings
    /// from their linked rulings using heuristics (no LLM). Idempotent.
    /// </summary>
    [HttpPost("backfill-fields")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(BackfillProceedingFieldsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<BackfillProceedingFieldsResult>> BackfillProceedingFields(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new BackfillProceedingFieldsCommand(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin: deletes all prosecutor opinions so they can be re-extracted with the updated prompt.
    /// Call extract-prosecutor-opinions after this to re-populate.
    /// </summary>
    [HttpDelete("prosecutor-opinions")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DeleteProsecutorOpinionsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<DeleteProsecutorOpinionsResult>> DeleteProsecutorOpinions(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProsecutorOpinionsCommand(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin: extracts prosecutor opinions from CSJN rulings via GPT.
    /// </summary>
    [HttpPost("extract-prosecutor-opinions")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ExtractProsecutorOpinionsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExtractProsecutorOpinionsResult>> ExtractProsecutorOpinions(
        [FromQuery] int batchSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExtractProsecutorOpinionsCommand(batchSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin: extracts proceeding parties (plaintiff/defendant) from case titles.
    /// Uses heuristic regex first, falls back to GPT-5 nano for ambiguous carátulas.
    /// </summary>
    [HttpPost("extract-parties")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ExtractPartiesResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExtractPartiesResult>> ExtractParties(
        [FromQuery] int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExtractPartiesCommand(batchSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin: resolves unresolved citations (TargetRulingId IS NULL) using heuristics.
    /// Strategy 1: match "Fallos 328:1883" against Sumario Volume+Page.
    /// Strategy 2: match normalized alias against CaseNumber/ExternalAlias.
    /// No LLM — pure SQL matching.
    /// </summary>
    [HttpPost("resolve-citations")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ResolveCitationsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResolveCitationsResult>> ResolveCitations(
        [FromQuery] int batchSize = 200,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ResolveCitationsCommand(batchSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin: extracts RatioDecidendi and DoctrinaLegal from rulings via GPT nano.
    /// Processes all rulings with FullText that don't have RatioDecidendi yet.
    /// </summary>
    [HttpPost("extract-doctrine")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ExtractDoctrineResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExtractDoctrineResult>> ExtractDoctrine(
        [FromQuery] int batchSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExtractDoctrineCommand(batchSize), cancellationToken);
        return Ok(result);
    }
}
