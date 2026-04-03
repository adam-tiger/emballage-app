namespace Phoenix.Application.Products.Dtos;

/// <summary>
/// Représente un palier tarifaire avec ses bornes de quantité et son prix unitaire HT.
/// </summary>
public sealed record PriceTierDto(
    Guid Id,
    int MinQuantity,
    int? MaxQuantity,
    decimal UnitPriceHT);
