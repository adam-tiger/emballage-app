using FluentValidation;

namespace Phoenix.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Valide <see cref="UpdateProductCommand"/> avant l'invocation du handler.
/// </summary>
public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    /// <summary>Initialise une nouvelle instance avec toutes les règles de validation.</summary>
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.NameFr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DescriptionFr)
            .MaximumLength(1000);
    }
}
