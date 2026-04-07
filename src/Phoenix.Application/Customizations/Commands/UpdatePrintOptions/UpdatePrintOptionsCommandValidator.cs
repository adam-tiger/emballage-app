using FluentValidation;

namespace Phoenix.Application.Customizations.Commands.UpdatePrintOptions;

/// <summary>
/// Validateur de la commande <see cref="UpdatePrintOptionsCommand"/>.
/// </summary>
public sealed class UpdatePrintOptionsCommandValidator
    : AbstractValidator<UpdatePrintOptionsCommand>
{
    /// <summary>
    /// Initialise les règles de validation de la commande de mise à jour des options d'impression.
    /// </summary>
    public UpdatePrintOptionsCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("L'identifiant du job de personnalisation est obligatoire.");

        RuleFor(x => x.PrintSide)
            .IsInEnum()
            .WithMessage("La valeur de face d'impression est invalide.");

        RuleFor(x => x.ColorCount)
            .IsInEnum()
            .WithMessage("La valeur du nombre de couleurs est invalide.");
    }
}
