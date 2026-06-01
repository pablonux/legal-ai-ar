using LegalAiAr.Api.Models;
using LegalAiAr.Application.Chat.Commands.ChatQuery;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers;

/// <summary>
/// Chat API: agentic jurisprudential assistant with SSE streaming.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Streams an agentic jurisprudential response as Server-Sent Events.
    /// Text chunks: <c>data: {text}\n\n</c>. Tool events: <c>event: tool_start/tool_end</c>.
    /// Stream ends with <c>data: [DONE]\n\n</c>.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task StreamChat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        var query = request.Query?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(query))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsJsonAsync(new { error = "La consulta no puede estar vacía." }, cancellationToken);
            return;
        }

        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("X-Accel-Buffering", "no");
        Response.Headers.Append("Connection", "keep-alive");

        var command = new ChatQueryCommand(query);

        using var streamCts = new CancellationTokenSource();

        try
        {
            await foreach (var evt in _mediator.CreateStream(command, streamCts.Token))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var payload = evt switch
                {
                    ChatTextChunk text =>
                        "data: " + text.Text.Replace("\n", "\ndata: ") + "\n\n",
                    ChatToolStart toolStart =>
                        $"event: tool_start\ndata: {{\"tool\":\"{Escape(toolStart.ToolName)}\"}}\n\n",
                    ChatToolEnd toolEnd =>
                        $"event: tool_end\ndata: {{\"tool\":\"{Escape(toolEnd.ToolName)}\",\"resultCount\":{toolEnd.ResultCount}}}\n\n",
                    ChatValidationEvent validation =>
                        $"event: validation\ndata: {{\"status\":\"{Escape(validation.Status)}\",\"citationsChecked\":{validation.CitationsChecked},\"valid\":{validation.Valid},\"warnings\":{validation.Warnings},\"details\":[{string.Join(",", validation.Details.Select(d => $"\"{Escape(d)}\""))}]}}\n\n",
                    ChatNormalizedResponse normalized =>
                        "event: normalized\ndata: " + normalized.Text.Replace("\n", "\ndata: ") + "\n\n",
                    _ => null
                };

                if (payload is null) continue;

                await Response.Body.WriteAsync(
                    System.Text.Encoding.UTF8.GetBytes(payload), CancellationToken.None);
                await Response.Body.FlushAsync(CancellationToken.None);
            }

            await Response.Body.WriteAsync(
                System.Text.Encoding.UTF8.GetBytes("data: [DONE]\n\n"), CancellationToken.None);
            await Response.Body.FlushAsync(CancellationToken.None);
        }
        catch (OperationCanceledException) { /* client disconnected */ }
    }

    private static string Escape(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
