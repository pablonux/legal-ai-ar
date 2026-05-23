using LegalAiAr.Application.Admin.RulingReprocess.Commands.EnqueueRulingReprocess;
using LegalAiAr.Application.Admin.RulingReprocess.Commands.RetryRulingReprocess;
using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Admin.RulingReprocess.Queries.ListRulingReprocessRequests;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/ruling-reprocess")]
[Authorize(Policy = "AdminOnly")]
public class RulingReprocessAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public RulingReprocessAdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(RulingReprocessListResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<RulingReprocessListResult>> List(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        RulingReprocessRequestStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<RulingReprocessRequestStatus>(status, true, out var parsed))
            statusFilter = parsed;

        var result = await _mediator.Send(
            new ListRulingReprocessRequestsQuery(statusFilter, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("rulings/{rulingId:guid}")]
    [ProducesResponseType(typeof(EnqueueRulingReprocessResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnqueueRulingReprocessResult>> Enqueue(
        Guid rulingId,
        [FromBody] EnqueueRulingReprocessRequest? body,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new EnqueueRulingReprocessCommand(
                rulingId,
                GetRequesterEmail(),
                body?.UseCache ?? false),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{requestId:guid}/retry")]
    [ProducesResponseType(typeof(EnqueueRulingReprocessResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<EnqueueRulingReprocessResult>> Retry(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RetryRulingReprocessCommand(requestId, GetRequesterEmail()),
            cancellationToken);
        return Ok(result);
    }

    private string GetRequesterEmail() =>
        User.Identity?.Name
        ?? User.FindFirst("preferred_username")?.Value
        ?? User.FindFirst("email")?.Value
        ?? "admin";
}

public record EnqueueRulingReprocessRequest(bool UseCache = false);
