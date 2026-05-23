using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Statutes.DTOs;
using LegalAiAr.Application.Statutes.Queries.GetStatuteById;
using LegalAiAr.Application.Statutes.Queries.GetStatutes;
using LegalAiAr.Application.Statutes.Queries.GetStatutePyramid;
using LegalAiAr.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatutesController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatutesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(StatutePageDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StatutePageDto>> List(
        [FromQuery(Name = "q")] string? search = null,
        [FromQuery] NormType? normType = null,
        [FromQuery] NormativeLevel? normativeLevel = null,
        [FromQuery] LegalBranch? legalBranch = null,
        [FromQuery] bool? isVigente = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetStatutesQuery(search, normType, normativeLevel, legalBranch, isVigente, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("pyramid")]
    [ProducesResponseType(typeof(IReadOnlyList<PyramidLevelDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PyramidLevelDto>>> Pyramid(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetStatutePyramidQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StatuteDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StatuteDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetStatuteByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
