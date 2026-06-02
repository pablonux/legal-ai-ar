using System.Text;
using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Chat.Commands.ChatQuery;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Chat;

/// <summary>
/// Streams an agentic jurisprudential response as Server-Sent Events.
/// </summary>
public sealed class PostChatStream : IEndpoint
{
    public string GroupName => ChatEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(string.Empty, StreamChat)
        .WithName("PostChatStream")
        .WithTags("Chat")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

    private static async Task StreamChat(
        ChatRequest request,
        HttpContext httpContext,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = request.Query?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(query))
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(
                new { error = "La consulta no puede estar vacía." },
                cancellationToken);
            return;
        }

        httpContext.Response.Headers.Append("Content-Type", "text/event-stream");
        httpContext.Response.Headers.Append("Cache-Control", "no-cache");
        httpContext.Response.Headers.Append("X-Accel-Buffering", "no");
        httpContext.Response.Headers.Append("Connection", "keep-alive");

        var command = new ChatQueryCommand(query);

        using var streamCts = new CancellationTokenSource();

        try
        {
            await foreach (var evt in mediator.CreateStream(command, streamCts.Token))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var payload = ChatSseEventFormatter.Format(evt);
                if (payload is null)
                    continue;

                await httpContext.Response.Body.WriteAsync(
                    Encoding.UTF8.GetBytes(payload),
                    CancellationToken.None);
                await httpContext.Response.Body.FlushAsync(CancellationToken.None);
            }

            await httpContext.Response.Body.WriteAsync(
                Encoding.UTF8.GetBytes("data: [DONE]\n\n"),
                CancellationToken.None);
            await httpContext.Response.Body.FlushAsync(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // Client disconnected.
        }
    }
}

internal static class ChatEndpointGroup
{
    public const string Name = "chat";
}
