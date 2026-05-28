using System.Threading;
using LegalAiAr.Infrastructure.Crawling.Options;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Crawling;

/// <summary>
/// Process-wide coordination for CSJN sjconsulta HTTP: minimum spacing between request starts
/// (from <see cref="CsjnApiOptions.ThrottlingDelayMs"/>) and a bulkhead on concurrent in-flight requests
/// (<see cref="CsjnApiOptions.PostAbrirHttpMaxConcurrencyGlobal"/>).
/// </summary>
public sealed class CsjnApiRequestGate : IDisposable
{
    private readonly IOptions<CsjnApiOptions> _options;
    private readonly SemaphoreSlim _spacing = new(1, 1);
    private readonly SemaphoreSlim _inFlight;
    private DateTimeOffset _nextHttpStartUtc = DateTimeOffset.MinValue;

    public CsjnApiRequestGate(IOptions<CsjnApiOptions> options)
    {
        _options = options;
        var n = Math.Clamp(options.Value.PostAbrirHttpMaxConcurrencyGlobal, 1, 32);
        _inFlight = new SemaphoreSlim(n, n);
    }

    /// <summary>
    /// Waits for spacing and bulkhead, then returns a lease that must be disposed (async) after each HTTP attempt completes.
    /// </summary>
    public async ValueTask<CsjnHttpSlotLease> AcquireAsync(CancellationToken cancellationToken = default)
    {
        await _spacing.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var delayMs = Math.Max(0, _options.Value.ThrottlingDelayMs);
            if (delayMs > 0)
            {
                var now = DateTimeOffset.UtcNow;
                var wait = _nextHttpStartUtc - now;
                if (wait > TimeSpan.Zero)
                    await Task.Delay(wait, cancellationToken).ConfigureAwait(false);
                _nextHttpStartUtc = DateTimeOffset.UtcNow.AddMilliseconds(delayMs);
            }
        }
        finally
        {
            _spacing.Release();
        }

        await _inFlight.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new CsjnHttpSlotLease(_inFlight);
    }

    public void Dispose()
    {
        _spacing.Dispose();
        _inFlight.Dispose();
    }
}

/// <summary>Releases the in-flight bulkhead slot.</summary>
public sealed class CsjnHttpSlotLease : IAsyncDisposable
{
    private readonly SemaphoreSlim _inFlight;
    private int _disposed;

    internal CsjnHttpSlotLease(SemaphoreSlim inFlight) => _inFlight = inFlight;

    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
            _inFlight.Release();
        return ValueTask.CompletedTask;
    }
}
