using System.Runtime.CompilerServices;
using LegalAiAr.Application.Chat.Commands.ChatQuery;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Stream handler that wraps the agentic executor (<see cref="ChatQueryHandler"/>) with
/// pre-stream, chunk-mode, and post-stream pipeline stages.
/// When no stages are registered or enabled, it degrades to a transparent pass-through.
/// </summary>
public class ChatPipelineOrchestrator : IStreamRequestHandler<ChatQueryCommand, ChatStreamEvent>
{
    private readonly IEnumerable<IChatPipelineStage> _stages;
    private readonly ChatQueryHandler _executor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ChatPipelineOptions _options;
    private readonly ILogger<ChatPipelineOrchestrator> _logger;

    public ChatPipelineOrchestrator(
        IEnumerable<IChatPipelineStage> stages,
        ChatQueryHandler executor,
        IServiceProvider serviceProvider,
        IOptions<ChatPipelineOptions> options,
        ILogger<ChatPipelineOrchestrator> logger)
    {
        _stages = stages;
        _executor = executor;
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    public async IAsyncEnumerable<ChatStreamEvent> Handle(
        ChatQueryCommand request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var context = new ChatPipelineContext
        {
            OriginalQuery = request.Query,
            Options = _options,
            ToolContext = new ToolExecutionContext { Services = _serviceProvider }
        };

        var enabledStages = _stages
            .Where(s => s.IsEnabled(_options))
            .ToList();

        var preStreamStages = enabledStages
            .Where(s => s.Phase == ChatPipelinePhase.PreStream)
            .ToList();
        var chunkModeStages = enabledStages
            .Where(s => s.Phase == ChatPipelinePhase.ChunkMode)
            .ToList();
        var postStreamStages = enabledStages
            .Where(s => s.Phase == ChatPipelinePhase.PostStream)
            .ToList();

        if (chunkModeStages.Count > 0)
            _logger.LogWarning(
                "ChunkMode stages registered ({Count}) but not yet supported — they will be skipped. " +
                "Move logic to PostStream or wait for chunk-mode implementation",
                chunkModeStages.Count);

        _logger.LogInformation(
            "Pipeline executing with {Pre} pre-stream, {Chunk} chunk-mode, {Post} post-stream stages",
            preStreamStages.Count, chunkModeStages.Count, postStreamStages.Count);

        // Phase 1: Pre-stream stages (guardrails, enrichment)
        foreach (var stage in preStreamStages)
        {
            _logger.LogDebug("Running pre-stream stage: {Stage}", stage.Name);

            ChatPipelineResult result;
            try
            {
                result = await stage.ProcessAsync(context, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Pre-stream stage {Stage} failed", stage.Name);
                result = ChatPipelineResult.Continue();
            }

            if (result.ImmediateEvents is not null)
                foreach (var evt in result.ImmediateEvents)
                    yield return evt;

            if (!result.ShouldContinue)
            {
                _logger.LogInformation("Pipeline short-circuited by {Stage}", stage.Name);
                context.IsShortCircuited = true;
                yield break;
            }
        }

        // Phase 2: Agentic executor — pass enrichment messages and shared tool context
        var executorCommand = request with
        {
            PipelineMessages = context.Messages.Count > 0 ? context.Messages : null,
            PipelineToolContext = context.ToolContext
        };
        await foreach (var evt in _executor.Handle(executorCommand, cancellationToken))
        {
            if (evt is ChatTextChunk textChunk)
                context.AccumulatedResponse.Append(textChunk.Text);

            yield return evt;
        }

        // Phase 3: Post-stream stages (output guardrail, validation)
        foreach (var stage in postStreamStages)
        {
            _logger.LogDebug("Running post-stream stage: {Stage}", stage.Name);

            ChatPipelineResult result;
            try
            {
                result = await stage.ProcessAsync(context, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Post-stream stage {Stage} failed", stage.Name);
                continue;
            }

            if (result.ImmediateEvents is not null)
                foreach (var evt in result.ImmediateEvents)
                    yield return evt;
        }
    }
}
