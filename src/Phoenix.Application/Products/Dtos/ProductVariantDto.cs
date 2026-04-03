namespace Phoenix.Application.Products.Dtos;

/// <summary>
/// Représente une variante d'impression d'un produit avec ses paliers tarifaires.
/// </summary>
public sealed record ProductVariantDto(
    Guid Id,
    string Sku,
    string NameFr,
    int MinimumOrderQuantity,
    string PrintSide,
    string ColorCount,
    decimal PrintCoefficient,
    IReadOnlyList<PriceTierDto> PriceTiers);
