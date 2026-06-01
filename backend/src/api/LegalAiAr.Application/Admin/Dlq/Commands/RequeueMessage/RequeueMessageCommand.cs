using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Admin.Dlq.Commands.RequeueMessage;

/// <summary>
/// Command to requeue a message from DLQ to the origin queue.
/// </summary>
/// <param name="Queue">Queue name: crawler, parser, enrichment, or indexer.</param>
/// <param name="MessageId">Message ID as returned by GET /api/admin/dlq.</param>
public record RequeueMessageCommand(string Queue, string MessageId) : IRequest<RequeueResult>;
