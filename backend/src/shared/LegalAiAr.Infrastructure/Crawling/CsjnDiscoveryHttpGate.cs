using LegalAiAr.Infrastructure.Crawling.Options;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Crawling;

/// <summary>
/// Optional process-wide bulkhead for CSJN discovery HTTP (acuerdo init + pagination).
/// When <see cref="CsjnCrawlerOptions.DiscoveryHttpMaxConcurrency"/> is 0, <see cref="AcquireAsync"/> is a no-op.
/// </summary>
public sealed class CsjnDiscoveryHttpGate : IDisposable
{
    private readonly SemaphoreSlim? _inFlight;

    public CsjnDiscoveryHttpGate(IOptions<CsjnCrawlerOptions> options)
    {
        var n = Math.Clamp(options.Value.DiscoveryHttpMaxConcurrency, 0, 32);
        if (n > 0)
            _inFlight = new SemaphoreSlim(n, n);
    }

    public async ValueTask<IAsyncDisposable> AcquireAsync(CancellationToken cancellationToken = default)
    {
        if (_inFlight is null)
            return CsjnDiscoveryHttpNoOpLease.Instance;

        await _inFlight.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new CsjnDiscoveryHttpLease(_inFlight);
    }

    public void Dispose() => _inFlight?.Dispose();
}

internal sealed class CsjnDiscoveryHttpNoOpLease : IAsyncDisposable
{
    public static readonly CsjnDiscoveryHttpNoOpLease Instance = new();

    private CsjnDiscoveryHttpNoOpLease()
    {
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

internal sealed class CsjnDiscoveryHttpLease : IAsyncDisposable
{
    private readonly SemaphoreSlim _inFlight;
    private int _disposed;

    internal CsjnDiscoveryHttpLease(SemaphoreSlim inFlight) => _inFlight = inFlight;

    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
            _inFlight.Release();
        return ValueTask.CompletedTask;
    }
}
