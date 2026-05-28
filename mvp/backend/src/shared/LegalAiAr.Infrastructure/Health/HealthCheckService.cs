using Azure;
using Azure.Search.Documents;
using Azure.Storage.Blobs;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Infrastructure.Search;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Health;

/// <summary>
/// Performs health checks for SQL, Blob, and Azure AI Search.
/// </summary>
public class HealthCheckService : IHealthCheckService
{
    private const string Healthy = "Healthy";
    private const string Unhealthy = "Unhealthy";

    private readonly AppDbContext _dbContext;
    private readonly AzureBlobOptions _blobOptions;
    private readonly AzureSearchOptions _searchOptions;
    private readonly ILogger<HealthCheckService> _logger;

    public HealthCheckService(
        AppDbContext dbContext,
        IOptions<AzureBlobOptions> blobOptions,
        IOptions<AzureSearchOptions> searchOptions,
        ILogger<HealthCheckService> logger)
    {
        _dbContext = dbContext;
        _blobOptions = blobOptions.Value;
        _searchOptions = searchOptions.Value;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default)
    {
        var checks = new Dictionary<string, string>();

        var sqlOk = await CheckSqlAsync(cancellationToken);
        checks["sql"] = sqlOk ? Healthy : Unhealthy;

        var blobOk = await CheckBlobAsync(cancellationToken);
        checks["blob"] = blobOk ? Healthy : Unhealthy;

        var searchOk = await CheckSearchAsync(cancellationToken);
        checks["search"] = searchOk ? Healthy : Unhealthy;

        var status = DetermineOverallStatus(checks);
        return new HealthCheckResult(status, checks);
    }

    private async Task<bool> CheckSqlAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Database.CanConnectAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SQL health check failed");
            return false;
        }
    }

    private async Task<bool> CheckBlobAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_blobOptions.ConnectionString))
            return false;

        try
        {
            var blobServiceClient = new BlobServiceClient(_blobOptions.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_blobOptions.ContainerName);
            await containerClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Blob health check failed");
            return false;
        }
    }

    private async Task<bool> CheckSearchAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_searchOptions.Endpoint) || string.IsNullOrWhiteSpace(_searchOptions.ApiKey))
            return false;

        try
        {
            var credential = new AzureKeyCredential(_searchOptions.ApiKey);
            var searchClient = new SearchClient(new Uri(_searchOptions.Endpoint), _searchOptions.RulingIndexName, credential);
            await searchClient.GetDocumentCountAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Search health check failed");
            return false;
        }
    }

    private static string DetermineOverallStatus(IReadOnlyDictionary<string, string> checks)
    {
        var values = checks.Values.ToList();
        var unhealthyCount = values.Count(v => v == Unhealthy);

        if (unhealthyCount == 0)
            return Healthy;

        if (checks.GetValueOrDefault("sql") == Unhealthy)
            return Unhealthy;

        return "Degraded";
    }
}
