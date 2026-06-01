using FluentValidation;

namespace LegalAiAr.Application.Admin.Crawlers.Commands.UpdateCrawlerConfig;

/// <summary>
/// Validates UpdateCrawlerConfigCommand.
/// </summary>
public class UpdateCrawlerConfigCommandValidator : AbstractValidator<UpdateCrawlerConfigCommand>
{
    public UpdateCrawlerConfigCommandValidator()
    {
        RuleFor(x => x.SourceId)
            .InclusiveBetween(1, 6)
            .WithMessage("SourceId must be between 1 and 6 (CSJN=1, SAIJ=2, PJN=3, SCBA=4, SAIJ tesauro=6).");
    }
}
