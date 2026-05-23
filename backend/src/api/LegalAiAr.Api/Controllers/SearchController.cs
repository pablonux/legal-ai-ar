using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Search.DTOs;
using LegalAiAr.Application.Search.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(GlobalSearchResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<GlobalSearchResultDto>> GlobalSearch(
        [FromQuery(Name = "q")] string query,
        [FromQuery] int maxPerEntity = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GlobalSearchQuery(query, maxPerEntity),
            cancellationToken);
        return Ok(result);
    }
}
