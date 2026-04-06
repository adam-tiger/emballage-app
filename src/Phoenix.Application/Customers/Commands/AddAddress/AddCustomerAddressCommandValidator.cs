using FluentValidation;

namespace Phoenix.Application.Customers.Commands.AddAddress;

/// <summary>
/// Validateur FluentValidation pour <see cref="AddCustomerAddressCommand"/>.
/// </summary>
public sealed class AddCustomerAddressCommandValidator
    : AbstractValidator<AddCustomerAddressCommand>
{
    /// <summary>Initialise les règles de validation.</summary>
    public AddCustomerAddressCommandValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Le libellé de l'adresse est obligatoire.")
            .MaximumLength(100).WithMessage("Le libellé ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("La rue est obligatoire.")
            .MaximumLength(200).WithMessage("La rue ne peut pas dépasser 200 caractères.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ville est obligatoire.")
            .MaximumLength(100).WithMessage("La ville ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Le code postal est obligatoire.")
            .MaximumLength(10).WithMessage("Le code postal ne peut pas dépasser 10 caractères.")
            .Matches(@"^\d{5}$").WithMessage("Code postal invalide (5 chiffres).");

        RuleFor(x => x.Country)
            .MaximumLength(2).WithMessage("Le code pays ne peut pas dépasser 2 caractères.");
    }
}
