using FluentValidation;

namespace LegalAiAr.Application.Admin.Dlq.Commands.RequeueMessage;

/// <summary>
/// Validates RequeueMessageCommand.
/// </summary>
public class RequeueMessageCommandValidator : AbstractValidator<RequeueMessageCommand>
{
    private static readonly string[] ValidQueues = { "crawler", "parser", "enrichment", "indexer" };

    public RequeueMessageCommandValidator()
    {
        RuleFor(x => x.Queue)
            .NotEmpty()
            .WithMessage("Queue is required.")
            .Must(q => ValidQueues.Contains(q.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Queue must be one of: crawler, parser, enrichment, indexer.");

        RuleFor(x => x.MessageId)
            .NotEmpty()
            .WithMessage("MessageId is required.");
    }
}
