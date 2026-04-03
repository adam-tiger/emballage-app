using MediatR;

namespace Phoenix.Application.Products.Commands.AddPriceTier;

/// <summary>
/// Commande d'ajout d'un palier tarifaire à une variante de produit.
/// Les paliers sont gérés au niveau de la variante (<c>ProductVariant.AddPriceTier</c>).
/// Retourne le <see cref="Guid"/> du palier créé.
/// </summary>
public sealed record AddPriceTierCommand : IRequest<Guid>
{
    /// <summary>Identifiant du produit parent.</summary>
    public required Guid ProductId { get; init; }

    /// <summary>Identifiant de la variante à laquelle le palier est ajouté.</summary>
    public required Guid VariantId { get; init; }

    /// <summary>Quantité minimale inclusive à partir de laquelle ce palier s'applique.</summary>
    public required int MinQuantity { get; init; }

    /// <summary>
    /// Quantité maximale inclusive jusqu'à laquelle ce palier s'applique.
    /// <c>null</c> signifie « sans plafond » (dernier palier).
    /// </summary>
    public int? MaxQuantity { get; init; }

    /// <summary>Prix unitaire hors-taxes en EUR pour ce palier.</summary>
    public required decimal UnitPriceHT { get; init; }
}
