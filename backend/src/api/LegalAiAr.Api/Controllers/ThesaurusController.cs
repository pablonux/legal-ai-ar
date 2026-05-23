using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Thesaurus.DTOs;
using LegalAiAr.Application.Thesaurus.Queries.GetThesaurusById;
using LegalAiAr.Application.Thesaurus.Queries.GetThesaurusChildren;
using LegalAiAr.Application.Thesaurus.Queries.GetThesaurusRoots;
using LegalAiAr.Application.Thesaurus.Queries.SearchThesaurus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ThesaurusController : ControllerBase
{
    private readonly IMediator _mediator;

    public ThesaurusController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Autocomplete search for SAIJ thesaurus descriptors.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<ThesaurusTermDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ThesaurusTermDto>>> Search(
        [FromQuery(Name = "q")] string query,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return Ok(Array.Empty<ThesaurusTermDto>());

        var result = await _mediator.Send(
            new SearchThesaurusQuery(query.Trim(), limit), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Top-level thesaurus branches (depth 0).
    /// </summary>
    [HttpGet("roots")]
    [ProducesResponseType(typeof(IReadOnlyList<ThesaurusTermDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ThesaurusTermDto>>> GetRoots(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetThesaurusRootsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Narrower (child) terms for a given thesaurus term.
    /// </summary>
    [HttpGet("{id:int}/children")]
    [ProducesResponseType(typeof(IReadOnlyList<ThesaurusTermDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ThesaurusTermDto>>> GetChildren(
        int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetThesaurusChildrenQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get thesaurus term detail with all SKOS relations.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ThesaurusTermDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ThesaurusTermDetailDto>> GetById(
        int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetThesaurusByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
