using MediatR;
using Phoenix.Application.Customizations.Dtos;

namespace Phoenix.Application.Customizations.Commands.UploadCustomerLogo;

/// <summary>
/// Commande d'upload du logo client pour un job de personnalisation.
/// Le logo est transmis sous forme de flux binaire et sera stocké
/// dans le Blob Storage Azure via <c>IBlobStorageService</c>.
/// </summary>
public sealed record UploadCustomerLogoCommand : IRequest<LogoUploadResponse>
{
    /// <summary>Identifiant du job de personnalisation cible (obligatoire).</summary>
    public required Guid JobId { get; init; }

    /// <summary>Flux binaire du logo à uploader (SVG, PDF vectoriel, PNG ≥ 300 DPI, AI).</summary>
    public required Stream FileStream { get; init; }

    /// <summary>Nom de fichier original du logo (ex : <c>mon-logo-restaurant.svg</c>).</summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Type MIME du fichier logo.
    /// Valeurs acceptées : <c>image/svg+xml</c>, <c>application/pdf</c>,
    /// <c>image/png</c>, <c>application/postscript</c>.
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Identifiant du client demandeur, pour vérification d'ownership.
    /// <c>null</c> si la session est invitée.
    /// </summary>
    public Guid? CustomerId { get; init; }
}
