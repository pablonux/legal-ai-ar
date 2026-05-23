using LegalAiAr.Infrastructure;
using LegalAiAr.Infrastructure.Control;
using LegalAiAr.Worker.Indexer;
using LegalAiAr.Worker.Indexer.Chunking;
using LegalAiAr.Worker.Indexer.Extensions;
using LegalAiAr.Worker.Indexer.Steps;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLegalAiArInfrastructure(builder.Configuration);
builder.Services.AddSingleton<TextChunkingService>();
builder.Services.AddScoped<PersistRulingStep>();
builder.Services.AddScoped<UploadBlobStep>();
builder.Services.AddScoped<GenerateEmbeddingsStep>();
builder.Services.AddScoped<IndexSearchStep>();
builder.Services.AddScoped<ResolveCitationsStep>();
builder.Services.AddScoped<ExtractChunkMentionsStep>();
builder.Services.AddIndexerStrategies();
builder.Services.AddWorkerControlGate(builder.Configuration, "Indexer");
builder.Services.AddHostedService<IndexerWorkerService>();

var host = builder.Build();
host.Run();
