using FluentValidation;

namespace Phoenix.Application.Auth.Commands.ForgotPassword;

/// <summary>
/// Validateur FluentValidation pour <see cref="ForgotPasswordCommand"/>.
/// </summary>
public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    /// <summary>Initialise les règles de validation.</summary>
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'adresse e-mail est obligatoire.")
            .EmailAddress().WithMessage("L'adresse e-mail n'est pas valide.")
            .MaximumLength(256).WithMessage("L'adresse e-mail ne peut pas dépasser 256 caractères.");
    }
}
