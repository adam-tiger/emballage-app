using FluentValidation;

namespace Phoenix.Application.Customizations.Commands.CreateCustomizationJob;

/// <summary>
/// Validateur de la commande <see cref="CreateCustomizationJobCommand"/>.
/// </summary>
public sealed class CreateCustomizationJobCommandValidator
    : AbstractValidator<CreateCustomizationJobCommand>
{
    /// <summary>
    /// Initialise les règles de validation de la commande de création de job.
    /// </summary>
    public CreateCustomizationJobCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("L'identifiant du produit est obligatoire.");

        RuleFor(x => x.ProductVariantId)
            .NotEmpty()
            .WithMessage("L'identifiant de la variante produit est obligatoire.");
    }
}
