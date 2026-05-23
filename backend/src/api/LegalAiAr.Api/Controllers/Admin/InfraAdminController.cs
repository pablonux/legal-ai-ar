using LegalAiAr.Application.Admin.Infra.Queries.ProbeStorageHealth;
using LegalAiAr.Application.Mediation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers.Admin;

/// <summary>
/// Admin endpoints for infrastructure probes (Storage queues).
/// </summary>
[ApiController]
[Route("api/admin/infra")]
[Authorize]
public class InfraAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public InfraAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Probes Discoverer and Fetcher queue connectivity (GetProperties, no dequeue).
    /// </summary>
    [HttpGet("storage-probe")]
    [ProducesResponseType(typeof(ProbeStorageHealthResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProbeStorageHealthResultDto>> StorageProbe(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ProbeStorageHealthQuery(), cancellationToken);
        return Ok(result);
    }
}
