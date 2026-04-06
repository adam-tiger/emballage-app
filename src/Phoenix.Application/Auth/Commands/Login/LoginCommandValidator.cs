using FluentValidation;

namespace Phoenix.Application.Auth.Commands.Login;

/// <summary>
/// Validateur FluentValidation pour <see cref="LoginCommand"/>.
/// Validation légère — la vérification des credentials est dans le handler.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>Initialise les règles de validation.</summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'adresse e-mail est obligatoire.")
            .EmailAddress().WithMessage("L'adresse e-mail n'est pas valide.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.");
    }
}
