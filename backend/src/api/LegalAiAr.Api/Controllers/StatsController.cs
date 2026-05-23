using LegalAiAr.Application.Stats.DTOs;
using LegalAiAr.Application.Stats.Queries.GetKbStats;
using LegalAiAr.Application.Mediation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Returns aggregated knowledge base statistics.
    /// </summary>
    [HttpGet("kb")]
    [ProducesResponseType(typeof(KbStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<KbStatsDto>> GetKbStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKbStatsQuery(), cancellationToken);
        return Ok(result);
    }
}
