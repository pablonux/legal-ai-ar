using LegalAiAr.Api.Authorization;
using LegalAiAr.Api.Services;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LegalAiAr.Api.Hubs;

[Authorize(Policy = "WorkerControlHub")]
public class WorkerControlHub : Hub<IWorkerControlClient>
{
    private readonly ILogger<WorkerControlHub> _logger;
    private readonly IWorkerSignalRPresenceTracker _presence;
    private readonly InfraIncidentCoordinator _infraCoordinator;

    public WorkerControlHub(
        ILogger<WorkerControlHub> logger,
        IWorkerSignalRPresenceTracker presence,
        InfraIncidentCoordinator infraCoordinator)
    {
        _logger = logger;
        _presence = presence;
        _infraCoordinator = infraCoordinator;
    }

    public async Task ReportInfraIncident(InfraIncidentReport report)
    {
        _logger.LogWarning(
            "Infra incident from worker {WorkerType}: {Category} {ErrorCode} queue={Queue} job={JobId}",
            report.WorkerType, report.Category, report.ErrorCode, report.QueueName, report.IngestionJobId);
        await _infraCoordinator.HandleWorkerReportAsync(report, Context.ConnectionAborted);
    }

    public async Task JoinWorkerGroup(string workerType)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, workerType);
        _presence.RegisterConnection(Context.ConnectionId, workerType);
        _logger.LogInformation("Connection {ConnectionId} joined group {WorkerType}",
            Context.ConnectionId, workerType);
    }

    public async Task LeaveWorkerGroup(string workerType)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, workerType);
        _logger.LogInformation("Connection {ConnectionId} left group {WorkerType}",
            Context.ConnectionId, workerType);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Worker control client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _presence.UnregisterConnection(Context.ConnectionId);
        _logger.LogInformation("Worker control client disconnected: {ConnectionId}, reason: {Reason}",
            Context.ConnectionId, exception?.Message ?? "clean");
        await base.OnDisconnectedAsync(exception);
    }
}
