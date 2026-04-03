using FluentValidation;

namespace Phoenix.Application.Products.Commands.CreateProduct;

/// <summary>
/// Valide <see cref="CreateProductCommand"/> avant l'invocation du handler.
/// </summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>Initialise une nouvelle instance avec toutes les règles de validation.</summary>
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Z0-9\-]+$")
                .WithMessage("'{PropertyName}' doit contenir uniquement des majuscules, chiffres et tirets.");

        RuleFor(x => x.NameFr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DescriptionFr)
            .MaximumLength(1000);

        RuleFor(x => x.Family)
            .IsInEnum();
    }
}
