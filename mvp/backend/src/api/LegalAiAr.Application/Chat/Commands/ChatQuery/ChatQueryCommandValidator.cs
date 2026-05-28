using FluentValidation;

namespace LegalAiAr.Application.Chat.Commands.ChatQuery;

/// <summary>
/// Validates ChatQueryCommand. Query must not be null or empty.
/// </summary>
public class ChatQueryCommandValidator : AbstractValidator<ChatQueryCommand>
{
    public ChatQueryCommandValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .WithMessage("La consulta no puede estar vacía.");
    }
}
