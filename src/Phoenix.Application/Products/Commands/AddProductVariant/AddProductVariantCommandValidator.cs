using FluentValidation;

namespace Phoenix.Application.Products.Commands.AddProductVariant;

/// <summary>
/// Valide <see cref="AddProductVariantCommand"/> avant l'invocation du handler.
/// </summary>
public sealed class AddProductVariantCommandValidator : AbstractValidator<AddProductVariantCommand>
{
    /// <summary>Initialise une nouvelle instance avec toutes les règles de validation.</summary>
    public AddProductVariantCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.NameFr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.MinimumOrderQuantity)
            .GreaterThan(0);

        RuleFor(x => x.PrintSide)
            .IsInEnum();

        RuleFor(x => x.ColorCount)
            .IsInEnum();
    }
}
