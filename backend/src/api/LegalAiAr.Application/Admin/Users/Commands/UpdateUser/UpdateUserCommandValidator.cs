using FluentValidation;

namespace LegalAiAr.Application.Admin.Users.Commands.UpdateUser;

/// <summary>
/// Validates UpdateUserCommand.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private static readonly string[] ValidRoles = { "admin", "lawyer", "viewer" };

    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.DisplayName));

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .Must(r => ValidRoles.Contains(r.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Role must be one of: admin, lawyer, viewer.");
    }
}
