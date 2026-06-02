using FluentValidation;
using LegalAiAr.Api.Authorization;
using LegalAiAr.Api.Validators.Rulings;
using LegalAiAr.Api.Extensions;
using LegalAiAr.Api.Middleware;
using LegalAiAr.Api.Services;
using LegalAiAr.Application.Extensions;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<WorkerControlOptions>(
    builder.Configuration.GetSection(WorkerControlOptions.SectionName));
builder.Services.AddSingleton<IAuthorizationHandler, WorkerControlHubAuthorizationHandler>();
builder.Services.AddSingleton<PlatformUserJwtValidator>();
builder.Services.AddLegalAiArInfrastructure(builder.Configuration);
builder.Services.AddLegalAiArApplication();
builder.Services.AddValidatorsFromAssemblyContaining<SearchRulingsRequestValidator>();
builder.Services.Configure<LegalAiAr.Application.Chat.ChatToolsOptions>(
    builder.Configuration.GetSection(LegalAiAr.Application.Chat.ChatToolsOptions.SectionName));
builder.Services.Configure<LegalAiAr.Core.Interfaces.Pipeline.ChatPipelineOptions>(
    builder.Configuration.GetSection(LegalAiAr.Core.Interfaces.Pipeline.ChatPipelineOptions.SectionName));
builder.Services.AddLegalAiArEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Legal AI AR API",
        Version = "v1",
        Description = "Argentine legal AI platform — rulings search, chat, and ingestion. "
            + "Identity is provided by a validated GCaaS session JWT (id_token cookie).",
    });
});

// Platform authentication (GCaaS / Entra via Envoy-injected headers).
builder.Services.AddOptions<PlatformAuthOptions>()
    .Bind(builder.Configuration.GetSection(PlatformAuthOptions.SectionName))
    .PostConfigure(PlatformAuthConfigurer.Apply);
builder.Services.Configure<DevelopmentAuthOptions>(
    builder.Configuration.GetSection(DevelopmentAuthOptions.SectionName));

var platformAuth = builder.Configuration.GetSection(PlatformAuthOptions.SectionName).Get<PlatformAuthOptions>()
    ?? new PlatformAuthOptions();
PlatformAuthConfigurer.Apply(platformAuth);
PlatformAuthConfigurer.ValidateForStartup(platformAuth);

builder.Services.AddAuthentication(PlatformAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, PlatformAuthenticationHandler>(
        PlatformAuthenticationHandler.SchemeName, _ => { });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("WorkerControlHub", policy =>
        policy.Requirements.Add(new WorkerControlHubRequirement()));
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<InfraIncidentCoordinator>();
builder.Services.AddSingleton<IInfraRecoveryEvents, SignalRInfraRecoveryEvents>();
builder.Services.AddScoped<JobInfraRecoveryOrchestrator>();

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:4200", "https://legal-ai-ar.azurestaticapps.net"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "docs/{documentName}/swagger.json";
        options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/docs/v1/swagger.json", "Legal AI AR API v1");
        options.RoutePrefix = "docs";
    });
    app.UseDevelopmentPlatformAuth();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapLegalAiArEndpoints();
app.MapHub<LegalAiAr.Api.Hubs.WorkerControlHub>("/hubs/worker-control");

app.Run();
