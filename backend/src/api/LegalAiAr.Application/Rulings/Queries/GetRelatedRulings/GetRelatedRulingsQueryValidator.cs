using FluentValidation;

namespace LegalAiAr.Application.Rulings.Queries.GetRelatedRulings;

/// <summary>
/// Validator for GetRelatedRulingsQuery.
/// </summary>
public class GetRelatedRulingsQueryValidator : AbstractValidator<GetRelatedRulingsQuery>
{
    private const int MaxLimit = 20;

    public GetRelatedRulingsQueryValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, MaxLimit).WithMessage($"Limit must be between 1 and {MaxLimit}.");
    }
}
