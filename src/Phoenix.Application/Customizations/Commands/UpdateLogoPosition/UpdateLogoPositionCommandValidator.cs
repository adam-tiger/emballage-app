using FluentValidation;

namespace Phoenix.Application.Customizations.Commands.UpdateLogoPosition;

/// <summary>
/// Validateur de la commande <see cref="UpdateLogoPositionCommand"/>.
/// Les bornes reproduisent les invariants du value object <c>LogoPosition</c>
/// pour renvoyer des erreurs lisibles avant d'atteindre le domaine.
/// </summary>
public sealed class UpdateLogoPositionCommandValidator
    : AbstractValidator<UpdateLogoPositionCommand>
{
    /// <summary>
    /// Initialise les règles de validation de la commande de mise à jour de position.
    /// </summary>
    public UpdateLogoPositionCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("L'identifiant du job de personnalisation est obligatoire.");

        RuleFor(x => x.PositionX)
            .InclusiveBetween(0m, 100m)
            .WithMessage("PositionX doit être comprise entre 0 et 100 (% du canvas).");

        RuleFor(x => x.PositionY)
            .InclusiveBetween(0m, 100m)
            .WithMessage("PositionY doit être comprise entre 0 et 100 (% du canvas).");

        RuleFor(x => x.ScaleX)
            .InclusiveBetween(0.1m, 3.0m)
            .WithMessage("ScaleX doit être compris entre 0.1 et 3.0.");

        RuleFor(x => x.ScaleY)
            .InclusiveBetween(0.1m, 3.0m)
            .WithMessage("ScaleY doit être compris entre 0.1 et 3.0.");

        RuleFor(x => x.Rotation)
            .InclusiveBetween(-180m, 180m)
            .WithMessage("La rotation doit être comprise entre -180° et 180°.");
    }
}
