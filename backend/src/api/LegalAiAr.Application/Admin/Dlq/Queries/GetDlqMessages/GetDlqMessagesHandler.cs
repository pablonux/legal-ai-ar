using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Admin.Dlq.Queries.GetDlqMessages;

/// <summary>
/// Handles GetDlqMessagesQuery: peeks messages from the specified DLQ.
/// </summary>
public class GetDlqMessagesHandler : IRequestHandler<GetDlqMessagesQuery, DlqPeekResult>
{
    private readonly IDlqService _dlqService;

    public GetDlqMessagesHandler(IDlqService dlqService)
    {
        _dlqService = dlqService;
    }

    public async Task<DlqPeekResult> Handle(GetDlqMessagesQuery request, CancellationToken cancellationToken)
    {
        var queue = request.Queue.Trim().ToLowerInvariant();
        if (!_dlqService.ValidQueueNames.Contains(queue))
            throw new DomainException($"Invalid queue '{queue}'. Valid: {string.Join(", ", _dlqService.ValidQueueNames)}.");

        return await _dlqService.PeekMessagesAsync(
            queue,
            Math.Clamp(request.MaxMessages, 1, 32),
            cancellationToken);
    }
}
