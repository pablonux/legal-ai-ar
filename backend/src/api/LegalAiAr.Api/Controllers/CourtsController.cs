using LegalAiAr.Application.Catalogs.DTOs;
using LegalAiAr.Application.Catalogs.Queries.GetCourtById;
using LegalAiAr.Application.Catalogs.Queries.GetCourts;
using LegalAiAr.Application.Mediation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourtsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourtsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lists courts with optional search and filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CourtListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourtListItemDto>>> List(
        [FromQuery(Name = "q")] string? query = null,
        [FromQuery] string? jurisdictionArea = null,
        [FromQuery] string? instance = null,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetCourtsQuery(query, jurisdictionArea, instance, limit), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets court detail with stats and participating judges.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CourtDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourtDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCourtByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
