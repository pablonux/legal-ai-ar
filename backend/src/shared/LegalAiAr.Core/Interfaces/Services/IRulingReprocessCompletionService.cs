namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Called by pipeline workers when a document tied to an admin reprocess request finishes or fails.
/// </summary>
public interface IRulingReprocessCompletionService
{
    Task OnPipelineSucceededAsync(Guid pipelineDocumentId, CancellationToken cancellationToken = default);

    Task OnPipelineFailedAsync(Guid pipelineDocumentId, string errorMessage, CancellationToken cancellationToken = default);
}
