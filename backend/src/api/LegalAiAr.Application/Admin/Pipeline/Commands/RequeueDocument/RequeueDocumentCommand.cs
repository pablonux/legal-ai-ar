using System.Text.Json;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Pipeline.Commands.RequeueDocument;

/// <summary>
/// Re-queues a document to a pipeline stage. Either provide full message or rulingId for reconstruction.
/// </summary>
/// <param name="Stage">parser, enrichment, or indexer.</param>
/// <param name="Message">Full message payload as JSON (when client has it, e.g. from DLQ).</param>
/// <param name="RulingId">Ruling ID for backend reconstruction. Mutually exclusive with Message.</param>
public record RequeueDocumentCommand(
    string Stage,
    JsonElement? Message,
    Guid? RulingId) : IRequest<RequeueDocumentResult>;
