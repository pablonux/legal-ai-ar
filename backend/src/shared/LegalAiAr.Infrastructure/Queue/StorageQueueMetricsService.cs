using Azure.Storage.Queues;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Queue;

/// <summary>
/// Gets queue metrics from Azure Storage Queues.
/// </summary>
public class StorageQueueMetricsService : IQueueMetricsService
{
    private readonly StorageQueueOptions _options;

    public StorageQueueMetricsService(IOptions<StorageQueueOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<int> GetApproximateMessageCountAsync(string queueName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            return 0;

        try
        {
            var client = new QueueClient(_options.ConnectionString, queueName);
            await client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var response = await client.GetPropertiesAsync(cancellationToken);
            return response.Value.ApproximateMessagesCount;
        }
        catch
        {
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<(bool Ok, string? Error)> TryProbeQueueAsync(string queueName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            return (false, "Storage connection string is empty.");

        try
        {
            var client = new QueueClient(_options.ConnectionString, queueName);
            _ = await client.GetPropertiesAsync(cancellationToken);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
