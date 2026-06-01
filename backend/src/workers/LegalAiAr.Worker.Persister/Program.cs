using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure;
using LegalAiAr.Infrastructure.Control;
using LegalAiAr.Worker.Persister;
using LegalAiAr.Worker.Persister.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLegalAiArInfrastructure(builder.Configuration);
builder.Services.AddPersisterServices();
builder.Services.AddWorkerControlGate(builder.Configuration, "Persister");
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.AddHostedService<PersisterWorkerService>();

var host = builder.Build();
host.Run();
