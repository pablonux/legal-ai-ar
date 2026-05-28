using LegalAiAr.Application.Graph.Models;
using LegalAiAr.Application.Graph.Queries;
using LegalAiAr.Application.Mediation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/graph")]
[Authorize]
public class GraphExplorerController : ControllerBase
{
    private readonly IMediator _mediator;

    public GraphExplorerController(IMediator mediator) => _mediator = mediator;

    [HttpGet("neighborhood/{entityType}/{entityId}")]
    [ProducesResponseType(typeof(NeighborhoodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NeighborhoodResponse>> GetNeighborhood(
        string entityType, string entityId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetNeighborhoodQuery(entityType, entityId), cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(EntitySearchResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EntitySearchResponse>> SearchEntities(
        [FromQuery] string q, [FromQuery] string? types, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(new EntitySearchResponse([]));

        var result = await _mediator.Send(new SearchEntitiesQuery(q, types), cancellationToken);
        return Ok(result);
    }
}
