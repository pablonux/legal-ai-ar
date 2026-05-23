using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Infrastructure.RulingReprocess;

public sealed class RulingReprocessCompletionService : IRulingReprocessCompletionService
{
    private readonly IRulingReprocessRequestRepository _requests;
    private readonly IRulingRepository _rulings;

    public RulingReprocessCompletionService(
        IRulingReprocessRequestRepository requests,
        IRulingRepository rulings)
    {
        _requests = requests;
        _rulings = rulings;
    }

    public async Task OnPipelineSucceededAsync(Guid pipelineDocumentId, CancellationToken cancellationToken = default)
    {
        var request = await _requests.GetActiveByDocumentIdAsync(pipelineDocumentId, cancellationToken);
        if (request is null)
            return;

        request.Status = RulingReprocessRequestStatus.Completed;
        request.CompletedAt = DateTime.UtcNow;
        request.ErrorMessage = null;
        await _requests.SaveChangesAsync(cancellationToken);

        await _rulings.UpdateStatusAsync(request.RulingId, RulingStatus.Indexed, cancellationToken);
    }

    public async Task OnPipelineFailedAsync(Guid pipelineDocumentId, string errorMessage, CancellationToken cancellationToken = default)
    {
        var request = await _requests.GetActiveByDocumentIdAsync(pipelineDocumentId, cancellationToken);
        if (request is null)
            return;

        request.Status = RulingReprocessRequestStatus.Failed;
        request.CompletedAt = DateTime.UtcNow;
        request.ErrorMessage = errorMessage.Length > 4000 ? errorMessage[..4000] : errorMessage;
        await _requests.SaveChangesAsync(cancellationToken);

        await _rulings.UpdateStatusAsync(request.RulingId, RulingStatus.Error, cancellationToken);
    }
}
