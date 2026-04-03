using FluentValidation;

namespace Phoenix.Application.Products.Commands.AddPriceTier;

/// <summary>
/// Valide <see cref="AddPriceTierCommand"/> avant l'invocation du handler.
/// </summary>
public sealed class AddPriceTierCommandValidator : AbstractValidator<AddPriceTierCommand>
{
    /// <summary>Initialise une nouvelle instance avec toutes les règles de validation.</summary>
    public AddPriceTierCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.VariantId)
            .NotEmpty();

        RuleFor(x => x.MinQuantity)
            .GreaterThan(0);

        RuleFor(x => x.UnitPriceHT)
            .GreaterThan(0m);

        RuleFor(x => x.MaxQuantity)
            .GreaterThan(x => x.MinQuantity)
            .WithMessage("'MaxQuantity' doit être supérieur à 'MinQuantity'.")
            .When(x => x.MaxQuantity.HasValue);
    }
}
