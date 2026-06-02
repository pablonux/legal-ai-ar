using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Outbox;

/// <summary>
/// Polls <c>OutboxMessages</c> and dispatches domain event handlers (hosted in the API process).
/// </summary>
public sealed class OutboxDispatcherWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxDispatcherWorker> _logger;

    public OutboxDispatcherWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<OutboxOptions> options,
        ILogger<OutboxDispatcherWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Outbox dispatcher is disabled (Outbox:Enabled = false)");
            return;
        }

        _logger.LogInformation(
            "Outbox dispatcher started (poll every {PollIntervalSeconds}s, batch {BatchSize})",
            _options.PollIntervalSeconds,
            _options.BatchSize);

        var pollDelay = TimeSpan.FromSeconds(Math.Max(1, _options.PollIntervalSeconds));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var processor = scope.ServiceProvider.GetRequiredService<IOutboxMessageProcessor>();
                var processed = await processor.ProcessPendingAsync(stoppingToken).ConfigureAwait(false);

                if (processed > 0)
                {
                    _logger.LogDebug("Outbox dispatcher processed {Count} message(s)", processed);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox dispatcher poll failed");
            }

            try
            {
                await Task.Delay(pollDelay, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("Outbox dispatcher stopped");
    }
}
