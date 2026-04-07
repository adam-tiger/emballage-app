using FluentValidation;

namespace Phoenix.Application.Customizations.Commands.UploadCustomerLogo;

/// <summary>
/// Validateur de la commande <see cref="UploadCustomerLogoCommand"/>.
/// Le ContentType est re-validé ici même si l'extension a déjà été vérifiée
/// côté contrôleur, pour garantir la cohérence dans la couche Application.
/// </summary>
public sealed class UploadCustomerLogoCommandValidator
    : AbstractValidator<UploadCustomerLogoCommand>
{
    private static readonly string[] AcceptedContentTypes =
    [
        "image/svg+xml",
        "application/pdf",
        "image/png",
        "application/postscript"
    ];

    /// <summary>
    /// Initialise les règles de validation de la commande d'upload de logo.
    /// </summary>
    public UploadCustomerLogoCommandValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("L'identifiant du job de personnalisation est obligatoire.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("Le nom de fichier est obligatoire.")
            .MaximumLength(255)
            .WithMessage("Le nom de fichier ne peut pas dépasser 255 caractères.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Le type MIME du fichier est obligatoire.")
            .Must(ct => AcceptedContentTypes.Contains(ct))
            .WithMessage("Format accepté : SVG, PDF, PNG, AI");
    }
}
