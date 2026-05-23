using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure;
using LegalAiAr.Infrastructure.Control;
using LegalAiAr.Worker.Enrichment;
using LegalAiAr.Worker.Enrichment.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLegalAiArInfrastructure(builder.Configuration);
builder.Services.AddEnrichmentServices(builder.Configuration);
builder.Services.AddWorkerControlGate(builder.Configuration, "Enricher");
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.AddHostedService<EnrichmentWorkerService>();

var host = builder.Build();
host.Run();
