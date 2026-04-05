using FluentValidation;

namespace Phoenix.Application.Customers.Commands.UpdateProfile;

/// <summary>
/// Validateur FluentValidation pour <see cref="UpdateCustomerProfileCommand"/>.
/// </summary>
public sealed class UpdateCustomerProfileCommandValidator
    : AbstractValidator<UpdateCustomerProfileCommand>
{
    /// <summary>Initialise les règles de validation.</summary>
    public UpdateCustomerProfileCommandValidator()
    {
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
}
