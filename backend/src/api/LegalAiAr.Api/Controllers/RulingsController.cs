using LegalAiAr.Api.Models;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Rulings.Queries.GetRelatedRulings;
using LegalAiAr.Application.Rulings.Queries.GetRulingById;
using LegalAiAr.Application.Rulings.Queries.GetRulingDocument;
using LegalAiAr.Application.Rulings.Queries.GetSearchFacets;
using LegalAiAr.Application.Rulings.Queries.SearchRulings;
using LegalAiAr.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

/// <summary>
/// Rulings API: search, detail and related rulings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RulingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RulingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Hybrid semantic search over indexed rulings.
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType(typeof(SearchRulingsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearchRulingsResult>> Search(
        [FromBody] SearchRulingsRequest request,
        CancellationToken cancellationToken)
    {
        var filters = MapFilters(request.Filters);
        var query = new SearchRulingsQuery(
            request.Query,
            filters,
            request.Page,
            request.PageSize);

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Full ruling details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var allowReprocessing = User.IsInRole("admin");
        var query = new GetRulingByIdQuery(id, allowReprocessing);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Streams the original PDF document for a ruling.
    /// </summary>
    [HttpGet("{id:guid}/document")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocument(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRulingDocumentQuery(id), cancellationToken);
        if (result is null)
            return NotFound();

        return File(result.Content, result.ContentType, enableRangeProcessing: true);
    }

    /// <summary>
    /// Related rulings by semantic similarity.
    /// </summary>
    [HttpGet("{id:guid}/related")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRelated(
        Guid id,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var query = new GetRelatedRulingsQuery(id, limit ?? 10);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns facet values for search filter dropdowns.
    /// </summary>
    [HttpGet("facets")]
    [ProducesResponseType(typeof(SearchFacets), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchFacets>> GetFacets(CancellationToken cancellationToken)
    {
        var query = new GetSearchFacetsQuery();
        var facets = await _mediator.Send(query, cancellationToken);
        return Ok(facets);
    }

    private static SearchFilters? MapFilters(SearchFiltersRequest? request)
    {
        if (request is null)
            return null;

        return new SearchFilters(
            JurisdictionArea: request.JurisdictionArea,
            Instance: request.Instance,
            CourtId: request.CourtId,
            CourtName: request.Court,
            DateFrom: request.DateFrom,
            DateTo: request.DateTo,
            Keywords: request.Keywords,
            SubjectArea: request.SubjectArea,
            ResourceType: request.ResourceType,
            IsUnconstitutional: request.IsUnconstitutional);
    }
}
