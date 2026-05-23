namespace LegalAiAr.Core.Models;

/// <summary>
/// Execution phase of a chat pipeline stage relative to the agentic executor.
/// </summary>
public enum ChatPipelinePhase
{
    /// <summary>Runs before the agentic loop. Can short-circuit the pipeline.</summary>
    PreStream,

    /// <summary>Processes each <see cref="ChatStreamEvent"/> inline during streaming.</summary>
    ChunkMode,

    /// <summary>Runs after the agentic loop completes, on the accumulated response.</summary>
    PostStream
}
