using Azure.Storage.Blobs;

var connectionString = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString")
    ?? throw new InvalidOperationException("AzureBlob__ConnectionString environment variable is required.");

var containerName = Environment.GetEnvironmentVariable("AzureBlob__ContainerName")
    ?? "rulings-pdfs";

var client = new BlobServiceClient(connectionString);
var container = client.GetBlobContainerClient(containerName);

var exists = await container.ExistsAsync();
if (exists.Value)
{
    Console.WriteLine($"Container '{containerName}' already exists.");
    return 0;
}

await container.CreateIfNotExistsAsync();
Console.WriteLine($"Container '{containerName}' created successfully.");
return 0;
