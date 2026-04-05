using FluentValidation;

namespace Phoenix.Application.Auth.Commands.Register;

/// <summary>
/// Validateur FluentValidation pour <see cref="RegisterCommand"/>.
/// Vérifie le format de l'email, la robustesse du mot de passe et les champs obligatoires.
/// </summary>
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    /// <summary>Initialise toutes les règles de validation.</summary>
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'adresse e-mail est obligatoire.")
            .EmailAddress().WithMessage("L'adresse e-mail n'est pas valide.")
            .MaximumLength(256).WithMessage("L'adresse e-mail ne peut pas dépasser 256 caractères.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.")
            .MinimumLength(8)
            .MaximumLength(100)
            .Must(BeAStrongPassword)
            .WithMessage(
                "Le mot de passe doit contenir au moins 8 caractères, " +
                "une majuscule, une minuscule et un chiffre.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Les mots de passe ne correspondent pas.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Le prénom est obligatoire.")
            .MaximumLength(100).WithMessage("Le prénom ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Le nom de famille est obligatoire.")
            .MaximumLength(100).WithMessage("Le nom de famille ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.CompanyName)
            .MaximumLength(200)
            .WithMessage("La raison sociale ne peut pas dépasser 200 caractères.")
            .When(x => x.CompanyName is not null);

        RuleFor(x => x.Segment)
            .IsInEnum().WithMessage("Le segment professionnel sélectionné est invalide.");
    }

    private static bool BeAStrongPassword(string password)
        => password.Any(char.IsUpper)
        && password.Any(char.IsLower)
        && password.Any(char.IsDigit);
}
