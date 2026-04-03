namespace Phoenix.Application.Products.Dtos;

/// <summary>
/// Représentation complète d'un produit utilisée sur la fiche produit.
/// Étend <see cref="ProductSummaryDto"/> avec la description, les variantes et les images.
/// </summary>
public sealed class ProductDetailDto : ProductSummaryDto
{
    /// <summary>Description longue en français (marketing, caractéristiques techniques).</summary>
    public string? DescriptionFr { get; set; }

    /// <summary>Variantes d'impression disponibles avec leurs paliers tarifaires.</summary>
    public IReadOnlyList<ProductVariantDto> Variants { get; set; } = [];

    /// <summary>Galerie d'images du produit.</summary>
    public IReadOnlyList<ProductImageDto> Images { get; set; } = [];
}
