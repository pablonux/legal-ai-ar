using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Control;

/// <summary>Registers SignalR-backed pause/resume (<see cref="WorkerControlService"/>) for pipeline workers.</summary>
public static class WorkerControlServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IWorkerGate"/> and starts <see cref="WorkerControlService"/> as a hosted service.
    /// Connects to API SignalR hub at <c>WorkerControl:ApiBaseUrl</c> (or <see cref="FallbackApiBase"/>).
    /// </summary>
    public static IServiceCollection AddWorkerControlGate(
        this IServiceCollection services,
        IConfiguration configuration,
        string workerType)
    {
        var apiBaseUrl = configuration["WorkerControl:ApiBaseUrl"]?.TrimEnd('/')
                         ?? FallbackApiBase;
        var hubAccessKey = configuration["WorkerControl:HubAccessKey"] ?? string.Empty;

        services.AddSingleton(sp =>
            new WorkerControlService(
                workerType,
                apiBaseUrl,
                hubAccessKey,
                sp.GetRequiredService<IServiceScopeFactory>(),
                sp.GetRequiredService<ILogger<WorkerControlService>>()));

        services.AddSingleton<IWorkerGate>(sp => sp.GetRequiredService<WorkerControlService>());
        services.AddSingleton<IWorkerInfraNotifier>(sp => sp.GetRequiredService<WorkerControlService>());
        services.AddHostedService(sp => sp.GetRequiredService<WorkerControlService>());
        return services;
    }

    /// <summary>Default when unset (local API).</summary>
    public const string FallbackApiBase = "http://localhost:5088";
}
