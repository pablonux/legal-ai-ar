using LegalAiAr.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

/// <summary>
/// Health checks. <c>GET /api/health/live</c> is anonymous for load balancers;
/// full dependency check requires authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;

    public HealthController(IHealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Minimal liveness probe (no dependencies). Public.
    /// </summary>
    [HttpGet("live")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Live() => Ok(new { status = "live" });

    /// <summary>
    /// Returns health status of SQL, Blob, and Search dependencies.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(HealthCheckResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _healthCheckService.CheckAsync(cancellationToken);

        if (result.Status == "Unhealthy")
            return StatusCode(503, result);

        return Ok(result);
    }
}
