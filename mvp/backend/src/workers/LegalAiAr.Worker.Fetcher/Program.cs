using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure;
using LegalAiAr.Infrastructure.Control;
using LegalAiAr.Infrastructure.Crawling;
using LegalAiAr.Worker.Fetcher;
using LegalAiAr.Worker.Fetcher.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLegalAiArInfrastructure(builder.Configuration);
builder.Services.AddCrawlerServices(builder.Configuration);
builder.Services.AddCsjnSjconsultaApiClient(builder.Configuration);
builder.Services.AddFetcherServices();
builder.Services.AddWorkerControlGate(builder.Configuration, "Fetcher");
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.AddHostedService<FetcherWorkerService>();

var host = builder.Build();
host.Run();
