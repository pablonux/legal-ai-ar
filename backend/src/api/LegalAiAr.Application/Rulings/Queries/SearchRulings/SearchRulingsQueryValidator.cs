using FluentValidation;

namespace LegalAiAr.Application.Rulings.Queries.SearchRulings;

/// <summary>
/// Validator for SearchRulingsQuery.
/// </summary>
public class SearchRulingsQueryValidator : AbstractValidator<SearchRulingsQuery>
{
    private const int MaxQueryLength = 1000;
    private const int MaxPageSize = 50;

    public SearchRulingsQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Query) || x.Filters != null)
            .WithMessage("Either a query or at least one filter must be provided.");

        RuleFor(x => x.Query)
            .MaximumLength(MaxQueryLength).WithMessage($"Query must not exceed {MaxQueryLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Query));

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaxPageSize).WithMessage($"PageSize must be between 1 and {MaxPageSize}.");

        When(x => x.Filters != null, () =>
        {
            RuleFor(x => x.Filters!.DateFrom)
                .LessThanOrEqualTo(x => x.Filters!.DateTo)
                .When(x => x.Filters!.DateFrom.HasValue && x.Filters!.DateTo.HasValue)
                .WithMessage("DateFrom must be before or equal to DateTo.");
        });
    }
}
