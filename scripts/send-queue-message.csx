#r "nuget: Azure.Storage.Queues, 12.19.1"

using Azure.Storage.Queues;

var connStr = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString")
    ?? throw new InvalidOperationException("AzureBlob__ConnectionString not set");
var queueName = args.Length > 0 ? args[0] : "csjn-ruling-crawler";
var json = args.Length > 1 ? args[1] : """{"sourceId":1,"documentType":"ruling","type":"fallos-destacados","maxDocuments":10,"useCache":true}""";

var client = new QueueClient(connStr, queueName);
await client.CreateIfNotExistsAsync();
var response = await client.SendMessageAsync(json);
Console.WriteLine($"Sent to {queueName}: {json}");
Console.WriteLine($"MessageId: {response.Value.MessageId}");
