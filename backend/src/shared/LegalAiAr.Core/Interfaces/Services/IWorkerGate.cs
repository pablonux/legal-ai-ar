namespace LegalAiAr.Core.Interfaces.Services;

public interface IWorkerGate
{
    bool IsPaused { get; }
    Task WaitIfPausedAsync(CancellationToken cancellationToken);
}
