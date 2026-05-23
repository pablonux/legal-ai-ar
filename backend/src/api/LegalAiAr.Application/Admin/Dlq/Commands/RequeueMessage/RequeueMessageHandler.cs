using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Dlq.Commands.RequeueMessage;

/// <summary>
/// Handles RequeueMessageCommand: requeues a message from DLQ to origin queue.
/// </summary>
public class RequeueMessageHandler : IRequestHandler<RequeueMessageCommand, RequeueResult>
{
    private readonly IDlqService _dlqService;

    public RequeueMessageHandler(IDlqService dlqService)
    {
        _dlqService = dlqService;
    }

    public async Task<RequeueResult> Handle(RequeueMessageCommand request, CancellationToken cancellationToken)
    {
        var queue = request.Queue.Trim().ToLowerInvariant();
        if (!_dlqService.ValidQueueNames.Contains(queue))
            throw new DomainException($"Invalid queue. Must be one of: crawler, parser, enrichment, indexer.");

        if (string.IsNullOrWhiteSpace(request.MessageId))
            throw new DomainException("MessageId is required.");

        return await _dlqService.RequeueMessageAsync(queue, request.MessageId.Trim(), cancellationToken);
    }
}
