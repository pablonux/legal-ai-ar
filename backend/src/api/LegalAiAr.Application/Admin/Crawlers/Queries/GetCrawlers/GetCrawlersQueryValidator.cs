using FluentValidation;

namespace LegalAiAr.Application.Admin.Crawlers.Queries.GetCrawlers;

/// <summary>
/// Validates GetCrawlersQuery. When SourceId is provided, must be 1-6 (6 = SAIJ tesauro API).
/// </summary>
public class GetCrawlersQueryValidator : AbstractValidator<GetCrawlersQuery>
{
    public GetCrawlersQueryValidator()
    {
        When(x => x.SourceId.HasValue, () =>
        {
            RuleFor(x => x.SourceId!.Value)
                .InclusiveBetween(1, 6)
                .WithMessage("SourceId must be between 1 and 6 (CSJN=1, SAIJ=2, PJN=3, SCBA=4, SAIJ tesauro=6).");
        });
    }
}
