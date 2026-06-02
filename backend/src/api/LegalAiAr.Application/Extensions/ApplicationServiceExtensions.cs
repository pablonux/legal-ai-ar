using FluentValidation;
using LegalAiAr.Application.Chat;
using LegalAiAr.Application.Chat.Commands.ChatQuery;
using LegalAiAr.Application.Chat.Pipeline;
using LegalAiAr.Application.Chat.Tools;
using LegalAiAr.Application.Common.Behaviors;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Ontology;
using LegalAiAr.Application.Rulings.Queries.SearchRulings;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Extensions;

/// <summary>
/// Extension methods for registering Application services.
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Adds custom Mediator, FluentValidation and pipeline behaviors.
    /// </summary>
    public static IServiceCollection AddLegalAiArApplication(this IServiceCollection services)
    {
        services.AddMediator(
            typeof(SearchRulingsHandler).Assembly,
            typeof(ValidationBehavior<,>),
            typeof(LoggingBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(SearchRulingsQueryValidator).Assembly);
        services.AddDomainEventHandlersFromAssembly(typeof(SearchRulingsHandler).Assembly);

        // Chat tools (T-06+)
        services.AddSingleton<IChatTool, SearchRulingsTool>();
        services.AddSingleton<IChatTool, GetRulingDetailTool>();
        services.AddSingleton<IChatTool, GetRulingCitationsTool>();
        services.AddSingleton<IChatTool, GetRelatedRulingsTool>();
        services.AddSingleton<IChatTool, SearchByStatuteTool>();
        services.AddSingleton<IChatTool, CountRulingsTool>();
        services.AddSingleton<IChatTool, ListCourtsTool>();
        services.AddSingleton<IChatTool, ListPersonsTool>();
        services.AddSingleton<IChatTool, GetCaseHistoryTool>();
        services.AddSingleton<IChatTool, SearchChunksTool>();
        services.AddSingleton<IChatTool, ExploreGraphTool>();
        services.AddSingleton<IChatTool, FindConnectionTool>();
        services.AddSingleton<IChatTool, SearchCommunitiesTool>();

        services.AddSingleton<IToolRegistry>(sp =>
            new ToolRegistry(
                sp.GetServices<IChatTool>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ToolRegistry>>()));

        // Chat pipeline (F1-18): agentic executor registered explicitly
        // since the orchestrator is now the mediator stream handler.
        services.AddScoped<ChatQueryHandler>();

        // Pipeline stages (registered in execution order)
        services.AddSingleton<RuleBasedGuardrailClassifier>();
        services.AddSingleton<IGuardrailTemplateProvider, GuardrailTemplateProvider>();
        services.AddSingleton<IChatPipelineStage, InputGuardrailStage>();

        services.AddSingleton<RuleBasedQueryEnricher>();
        services.AddSingleton<OntologyContextProvider>();
        services.AddSingleton<IChatPipelineStage, QueryEnricherStage>();

        // Post-stream stages
        services.AddScoped<IChatPipelineStage, OutputGuardrailStage>();
        services.AddSingleton<IChatPipelineStage, ResponseFinalizerStage>();

        // Ontology model (in-memory, singleton)
        services.AddSingleton<OntologyModelProvider>();

        return services;
    }
}
