using LegalAiAr.Application.Admin.RulingReprocess.Commands.EnqueueRulingReprocess;
using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Admin.RulingReprocess.Commands.RetryRulingReprocess;

public sealed class RetryRulingReprocessHandler : IRequestHandler<RetryRulingReprocessCommand, EnqueueRulingReprocessResult>
{
    private readonly IRulingReprocessRequestRepository _requests;
    private readonly IMediator _mediator;

    public RetryRulingReprocessHandler(IRulingReprocessRequestRepository requests, IMediator mediator)
    {
        _requests = requests;
        _mediator = mediator;
    }

    public async Task<EnqueueRulingReprocessResult> Handle(
        RetryRulingReprocessCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await _requests.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new NotFoundException("Reprocess request not found.");

        if (existing.Status != RulingReprocessRequestStatus.Failed)
            throw new DomainException("Only failed requests can be retried.");

        existing.RetryCount++;
        await _requests.SaveChangesAsync(cancellationToken);

        return await _mediator.Send(
            new EnqueueRulingReprocessCommand(existing.RulingId, request.RequestedBy, existing.UseCache),
            cancellationToken);
    }
}
