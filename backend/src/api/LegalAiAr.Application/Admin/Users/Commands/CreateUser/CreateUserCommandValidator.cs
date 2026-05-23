using FluentValidation;

namespace LegalAiAr.Application.Admin.Users.Commands.CreateUser;

/// <summary>
/// Validates CreateUserCommand.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private static readonly string[] ValidRoles = { "admin", "lawyer", "viewer" };

    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(256);

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
