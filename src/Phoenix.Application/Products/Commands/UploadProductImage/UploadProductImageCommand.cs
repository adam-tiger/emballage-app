using MediatR;
using Phoenix.Application.Products.Dtos;

namespace Phoenix.Application.Products.Commands.UploadProductImage;

/// <summary>
/// Commande d'upload d'une image pour un produit.
/// Retourne une <see cref="UploadProductImageResponse"/> avec les URLs CDN de l'image.
/// </summary>
public sealed record UploadProductImageCommand : IRequest<UploadProductImageResponse>
{
    /// <summary>Identifiant du produit auquel l'image appartient.</summary>
    public required Guid ProductId { get; init; }

    /// <summary>Flux binaire du fichier image à uploader.</summary>
    public required Stream FileStream { get; init; }

    /// <summary>Nom de fichier original (utilisé pour détecter l'extension).</summary>
    public required string FileName { get; init; }

    /// <summary>MIME content-type de l'image (ex : "image/jpeg").</summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Indique si l'image uploadée doit devenir l'image principale du produit.
    /// Défaut : <c>true</c>.
    /// </summary>
    public bool SetAsMain { get; init; } = true;
}
