using FluentValidation;

namespace LegalAiAr.Application.Admin.Pipeline.Commands.RequeueDocument;

public class RequeueDocumentCommandValidator : AbstractValidator<RequeueDocumentCommand>
{
    private static readonly HashSet<string> ValidStages = ["parser", "enrichment", "indexer"];

    public RequeueDocumentCommandValidator()
    {
        RuleFor(x => x.Stage)
            .NotEmpty()
            .Must(s => ValidStages.Contains(s.ToLowerInvariant()))
            .WithMessage("Stage must be one of: parser, enrichment, indexer.");

        RuleFor(x => x)
            .Must(c => c.Message.HasValue != c.RulingId.HasValue)
            .WithMessage("Either 'message' or 'rulingId' must be provided, not both.");
    }
}
