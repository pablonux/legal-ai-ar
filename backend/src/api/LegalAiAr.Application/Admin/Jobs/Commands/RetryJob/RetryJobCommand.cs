using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Jobs.Commands.RetryJob;

/// <summary>
/// Re-queues a partial or failed job to queue-crawler using the original parameters.
/// </summary>
public record RetryJobCommand(Guid JobId) : IRequest<RetryJobResult>;

public record RetryJobResult(bool Success, string Message, Guid? NewJobId = null);
