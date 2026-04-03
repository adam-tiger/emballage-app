namespace Phoenix.Application.Products.Dtos;

/// <summary>
/// Réponse retournée après l'upload réussi d'une image produit vers le blob storage.
/// </summary>
/// <param name="ProductId">Identifiant du produit auquel l'image appartient.</param>
/// <param name="ImageId">Identifiant de la nouvelle entité <c>ProductImage</c>.</param>
/// <param name="MainUrl">URL CDN publique de l'image uploadée.</param>
/// <param name="ThumbUrl">URL CDN de la miniature, si générée.</param>
public sealed record UploadProductImageResponse(
    Guid ProductId,
    Guid ImageId,
    string MainUrl,
    string? ThumbUrl);
