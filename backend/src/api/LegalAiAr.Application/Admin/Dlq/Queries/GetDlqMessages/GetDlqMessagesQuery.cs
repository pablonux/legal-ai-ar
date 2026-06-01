using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Admin.Dlq.Queries.GetDlqMessages;

/// <summary>
/// Query to list messages in a DLQ.
/// </summary>
/// <param name="Queue">Queue name: crawler, parser, enrichment, or indexer.</param>
/// <param name="MaxMessages">Max messages to peek (1-32). Default 32.</param>
public record GetDlqMessagesQuery(string Queue, int MaxMessages = 32) : IRequest<DlqPeekResult>;
