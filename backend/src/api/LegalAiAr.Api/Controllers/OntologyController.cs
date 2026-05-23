using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology.Models;
using LegalAiAr.Application.Ontology.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OntologyController : ControllerBase
{
    private readonly IMediator _mediator;

    public OntologyController(IMediator mediator) => _mediator = mediator;

    [HttpGet("classes")]
    [ProducesResponseType(typeof(OntologyClassesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<OntologyClassesResponse>> GetClasses(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOntologyClassesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("graph")]
    [ProducesResponseType(typeof(OntologyGraphResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<OntologyGraphResponse>> GetGraph(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOntologyGraphQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(OntologyStatsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<OntologyStatsResponse>> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOntologyStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("taxonomies/{taxonomyId}")]
    [ProducesResponseType(typeof(TaxonomyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaxonomyResponse>> GetTaxonomy(string taxonomyId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTaxonomyValuesQuery(taxonomyId), cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }
}
