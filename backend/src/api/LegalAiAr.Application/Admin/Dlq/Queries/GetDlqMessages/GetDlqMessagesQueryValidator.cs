using FluentValidation;

namespace LegalAiAr.Application.Admin.Dlq.Queries.GetDlqMessages;

/// <summary>
/// Validates GetDlqMessagesQuery.
/// </summary>
public class GetDlqMessagesQueryValidator : AbstractValidator<GetDlqMessagesQuery>
{
    private static readonly string[] ValidQueues = { "crawler", "parser", "enrichment", "indexer" };

    public GetDlqMessagesQueryValidator()
    {
        RuleFor(x => x.Queue)
            .NotEmpty()
            .WithMessage("Queue is required.")
            .Must(q => ValidQueues.Contains(q.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Queue must be one of: crawler, parser, enrichment, indexer.");

        RuleFor(x => x.MaxMessages)
            .InclusiveBetween(1, 32)
            .WithMessage("MaxMessages must be between 1 and 32.");
    }
}
