namespace LegalAiAr.Core.Models;

/// <summary>
/// Result returned by each <see cref="LegalAiAr.Core.Interfaces.Pipeline.IChatPipelineStage"/>.
/// </summary>
/// <param name="ShouldContinue">
/// <c>true</c>: pipeline proceeds to the next stage or the executor.
/// <c>false</c>: pipeline short-circuits; <paramref name="ImmediateEvents"/> are streamed as the response.
/// </param>
/// <param name="ImmediateEvents">
/// Events to stream immediately to the client. Used for rejection templates (input guardrail)
/// and post-stream appends (output guardrail, disclaimer).
/// </param>
public sealed record ChatPipelineResult(
    bool ShouldContinue,
    IReadOnlyList<ChatStreamEvent>? ImmediateEvents = null)
{
    public static ChatPipelineResult Continue() => new(true);

    public static ChatPipelineResult ShortCircuit(params ChatStreamEvent[] events) =>
        new(false, events);

    public static ChatPipelineResult Append(params ChatStreamEvent[] events) =>
        new(true, events);
}
