// Requires NuGet package: Microsoft.AspNetCore.SignalR.Client
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace LegalAiAr.Infrastructure.Control;

public class WorkerControlService : IHostedService, IWorkerGate, IWorkerInfraNotifier, IDisposable
{
    private const string HubAccessKeyHeaderName = "X-Worker-Hub-Key";

    private readonly string _workerType;
    private readonly string _apiBaseUrl;
    private readonly string _hubAccessKey;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WorkerControlService> _logger;
    private readonly AsyncManualResetEvent _gate = new(initialState: true);
    private HubConnection? _connection;
    private volatile bool _isPaused;
    private long _lastInfraNotifyUtcMs;

    public WorkerControlService(
        string workerType,
        string apiBaseUrl,
        string hubAccessKey,
        IServiceScopeFactory scopeFactory,
        ILogger<WorkerControlService> logger)
    {
        _workerType = workerType;
        _apiBaseUrl = apiBaseUrl.TrimEnd('/');
        _hubAccessKey = hubAccessKey ?? string.Empty;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public bool IsPaused => _isPaused;

    public async Task WaitIfPausedAsync(CancellationToken cancellationToken)
    {
        if (!_isPaused) return;

        _logger.LogInformation("Worker {WorkerType} is paused — waiting for resume signal", _workerType);
        await _gate.WaitAsync(cancellationToken);
        _logger.LogInformation("Worker {WorkerType} resumed", _workerType);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await LoadInitialStateAsync(cancellationToken);

        _connection = new HubConnectionBuilder()
            .WithUrl($"{_apiBaseUrl}/hubs/worker-control", options =>
            {
                if (!string.IsNullOrEmpty(_hubAccessKey))
                    options.Headers.Add(HubAccessKeyHeaderName, _hubAccessKey);
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On("PauseAsync", () =>
        {
            _isPaused = true;
            _gate.Reset();
            _logger.LogInformation("Pause command received for {WorkerType}", _workerType);
        });

        _connection.On("ResumeAsync", () =>
        {
            _isPaused = false;
            _gate.Set();
            _logger.LogInformation("Resume command received for {WorkerType}", _workerType);
        });

        _connection.Reconnected += async _ =>
        {
            _logger.LogInformation("Reconnected to worker control hub, rejoining group {WorkerType}", _workerType);
            await _connection.InvokeAsync("JoinWorkerGroup", _workerType, cancellationToken);
        };

        _connection.Closed += ex =>
        {
            _logger.LogWarning(ex, "Worker control hub connection closed for {WorkerType}", _workerType);
            return Task.CompletedTask;
        };

        await _connection.StartAsync(cancellationToken);
        await _connection.InvokeAsync("JoinWorkerGroup", _workerType, cancellationToken);

        _logger.LogInformation(
            "WorkerControlService started for {WorkerType}, initial paused = {IsPaused}",
            _workerType, _isPaused);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        _logger.LogInformation("WorkerControlService stopped for {WorkerType}", _workerType);
    }

    /// <inheritdoc />
    public async ValueTask NotifyInfrastructureIncidentAsync(
        string queueName,
        string? errorCode,
        string? detail,
        Guid? ingestionJobId = null,
        CancellationToken cancellationToken = default)
    {
        var connection = _connection;
        if (connection?.State != HubConnectionState.Connected)
            return;

        var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var last = Interlocked.Read(ref _lastInfraNotifyUtcMs);
        if (nowMs - last < 20_000 && last != 0)
            return;

        Interlocked.Exchange(ref _lastInfraNotifyUtcMs, nowMs);

        try
        {
            var report = new InfraIncidentReport(
                "storage",
                errorCode ?? "",
                queueName,
                _workerType,
                detail,
                ingestionJobId);
            await connection.InvokeAsync("ReportInfraIncident", report, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ReportInfraIncident failed for queue {Queue}", queueName);
        }
    }

    public void Dispose()
    {
        _connection?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    private async Task LoadInitialStateAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var state = await db.Set<WorkerPauseState>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.WorkerType == _workerType, cancellationToken);

        _isPaused = state?.IsPaused ?? false;

        if (_isPaused)
            _gate.Reset();
    }

    private sealed class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> _tcs;

        public AsyncManualResetEvent(bool initialState)
        {
            _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (initialState)
                _tcs.TrySetResult(true);
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            return _tcs.Task.WaitAsync(cancellationToken);
        }

        public void Set()
        {
            _tcs.TrySetResult(true);
        }

        public void Reset()
        {
            var current = _tcs;
            if (current.Task.IsCompleted)
                Interlocked.CompareExchange(ref _tcs, new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously), current);
        }
    }
}
