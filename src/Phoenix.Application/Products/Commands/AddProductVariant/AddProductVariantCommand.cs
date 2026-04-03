using MediatR;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Products.Commands.AddProductVariant;

/// <summary>
/// Commande d'ajout d'une variante d'impression à un produit existant.
/// Retourne le <see cref="Guid"/> de la variante créée.
/// </summary>
public sealed record AddProductVariantCommand : IRequest<Guid>
{
    /// <summary>Identifiant du produit parent.</summary>
    public required Guid ProductId { get; init; }

    /// <summary>Référence SKU de la variante (unique au sein du produit, majuscules).</summary>
    public required string Sku { get; init; }

    /// <summary>Libellé commercial en français de la variante.</summary>
    public required string NameFr { get; init; }

    /// <summary>Quantité minimale de commande (MOQ) pour cette variante.</summary>
    public required int MinimumOrderQuantity { get; init; }

    /// <summary>Face(s) imprimée(s) : recto uniquement ou recto-verso.</summary>
    public required PrintSide PrintSide { get; init; }

    /// <summary>Nombre de couleurs d'impression.</summary>
    public required ColorCount ColorCount { get; init; }
}
