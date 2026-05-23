using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure;
using LegalAiAr.Infrastructure.Control;
using LegalAiAr.Worker.Parser;
using LegalAiAr.Worker.Parser.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLegalAiArInfrastructure(builder.Configuration);
builder.Services.AddParserServices(builder.Configuration);
builder.Services.AddWorkerControlGate(builder.Configuration, "Parser");
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.AddHostedService<ParserWorkerService>();

var host = builder.Build();
host.Run();
