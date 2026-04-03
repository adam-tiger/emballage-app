namespace Phoenix.Application.Products.Dtos;

/// <summary>
/// Représente une image produit avec ses URLs CDN principale et miniature.
/// </summary>
public sealed record ProductImageDto(
    Guid Id,
    string PublicUrl,
    string? ThumbPublicUrl,
    bool IsMain);
