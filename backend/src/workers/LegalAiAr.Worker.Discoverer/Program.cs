using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure;
using LegalAiAr.Infrastructure.Control;
using LegalAiAr.Infrastructure.Crawling;
using LegalAiAr.Worker.Discoverer;
using LegalAiAr.Worker.Discoverer.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLegalAiArInfrastructure(builder.Configuration);
builder.Services.AddCrawlerServices(builder.Configuration);
builder.Services.AddDiscovererServices();
builder.Services.AddWorkerControlGate(builder.Configuration, "Discoverer");
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.AddHostedService<DiscovererWorkerService>();

var host = builder.Build();
host.Run();
