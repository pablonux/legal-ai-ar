using FluentValidation;

namespace LegalAiAr.Application.Admin.Pipeline.Commands.BulkRequeue;

public class BulkRequeueCommandValidator : AbstractValidator<BulkRequeueCommand>
{
    private static readonly HashSet<string> ValidStages = ["enrichment", "indexer"];

    public BulkRequeueCommandValidator()
    {
        RuleFor(x => x.Stage)
            .NotEmpty()
            .Must(s => ValidStages.Contains(s.ToLowerInvariant()))
            .WithMessage("Stage must be one of: enrichment, indexer.");

        RuleFor(x => x.BatchSize)
            .InclusiveBetween(1, 500)
            .WithMessage("BatchSize must be between 1 and 500.");
    }
}
