using LegalAiAr.Application.Admin.Dlq.Commands.RequeueMessage;
using LegalAiAr.Application.Admin.Dlq.Queries.GetDlqMessages;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers.Admin;

/// <summary>
/// Admin API for Dead Letter Queue management.
/// </summary>
[ApiController]
[Route("api/admin/dlq")]
[Authorize]
public class DlqAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public DlqAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List messages in a DLQ.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DlqPeekResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DlqPeekResult>> GetMessages(
        [FromQuery] string queue,
        [FromQuery] int maxMessages = 32,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queue))
            return BadRequest(new { error = "Queue is required." });

        var query = new GetDlqMessagesQuery(queue.Trim(), maxMessages);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Requeue a message from DLQ to the origin queue.
    /// </summary>
    [HttpPost("{queue}/{id}/requeue")]
    [ProducesResponseType(typeof(RequeueResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequeueResult>> Requeue(
        string queue,
        string id,
        CancellationToken cancellationToken = default)
    {
        var command = new RequeueMessageCommand(queue, id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
