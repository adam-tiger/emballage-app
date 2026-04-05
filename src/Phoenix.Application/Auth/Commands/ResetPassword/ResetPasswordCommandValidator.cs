using FluentValidation;

namespace Phoenix.Application.Auth.Commands.ResetPassword;

/// <summary>
/// Validateur FluentValidation pour <see cref="ResetPasswordCommand"/>.
/// </summary>
public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    /// <summary>Initialise les règles de validation.</summary>
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'adresse e-mail est obligatoire.")
            .EmailAddress().WithMessage("L'adresse e-mail n'est pas valide.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Le token de réinitialisation est obligatoire.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Le nouveau mot de passe est obligatoire.")
            .MinimumLength(8)
            .MaximumLength(100)
            .Must(BeAStrongPassword)
            .WithMessage(
                "Le mot de passe doit contenir au moins 8 caractères, " +
                "une majuscule, une minuscule et un chiffre.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Les mots de passe ne correspondent pas.");
    }

    private static bool BeAStrongPassword(string password)
        => password.Any(char.IsUpper)
        && password.Any(char.IsLower)
        && password.Any(char.IsDigit);
}
