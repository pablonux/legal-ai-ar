namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Service for health checks of critical dependencies.
/// </summary>
public interface IHealthCheckService
{
    /// <summary>
    /// Performs health checks for SQL, Blob, and Search.
    /// </summary>
    Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of health check. Status is Healthy if all pass; Degraded if some fail; Unhealthy if critical (SQL) fails.
/// </summary>
/// <param name="Status">Healthy, Degraded, or Unhealthy.</param>
/// <param name="Checks">Per-dependency status: sql, blob, search.</param>
public record HealthCheckResult(string Status, IReadOnlyDictionary<string, string> Checks);
