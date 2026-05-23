using FluentValidation;

namespace LegalAiAr.Application.Admin.Crawlers.Commands.RunCrawler;

/// <summary>
/// Validates RunCrawlerCommand. Type must be incremental or by-range.
/// For incremental with no LastCrawledAt, Since is required (validated in handler with config).
/// For by-range, DateFrom and DateTo are required and DateFrom must be &lt;= DateTo.
/// </summary>
public class RunCrawlerCommandValidator : AbstractValidator<RunCrawlerCommand>
{
    private static readonly string[] ValidTypes = { "incremental", "by-range", "fallos-destacados" };

    public RunCrawlerCommandValidator()
    {
        RuleFor(x => x.SourceId)
            .InclusiveBetween(1, 4)
            .WithMessage("SourceId must be between 1 and 4 (CSJN=1, SAIJ=2, PJN=3, SCBA=4).");

        RuleFor(x => x.DocumentType)
            .NotEmpty()
            .WithMessage("DocumentType is required (e.g. 'ruling').");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Type is required.")
            .Must(t => ValidTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Type must be 'incremental', 'by-range', or 'fallos-destacados'.");

        RuleFor(x => x.DateFrom)
            .NotNull()
            .When(x => x.Type.Equals("by-range", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DateFrom is required for by-range crawl.");

        RuleFor(x => x.DateTo)
            .NotNull()
            .When(x => x.Type.Equals("by-range", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DateTo is required for by-range crawl.");

        RuleFor(x => x)
            .Must(x => !x.DateFrom.HasValue || !x.DateTo.HasValue || x.DateFrom.Value <= x.DateTo.Value)
            .When(x => x.Type.Equals("by-range", StringComparison.OrdinalIgnoreCase) && x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateFrom must be less than or equal to DateTo.");

        RuleFor(x => x.MaxDocuments)
            .GreaterThan(0)
            .When(x => x.MaxDocuments.HasValue)
            .WithMessage("MaxDocuments must be greater than 0.");
    }
}
