using MediatR;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Products.Commands.CreateProduct;

/// <summary>
/// Commande de création d'un nouveau produit dans le catalogue.
/// Retourne le <see cref="Guid"/> du produit créé.
/// </summary>
public sealed record CreateProductCommand : IRequest<Guid>
{
    /// <summary>Référence SKU unique du produit (majuscules, alphanumérique + tiret).</summary>
    public required string Sku { get; init; }

    /// <summary>Libellé commercial en français.</summary>
    public required string NameFr { get; init; }

    /// <summary>Description longue en français (marketing, caractéristiques techniques).</summary>
    public required string DescriptionFr { get; init; }

    /// <summary>Famille de produits du catalogue Phoenix.</summary>
    public required ProductFamily Family { get; init; }

    /// <summary>Le produit accepte une personnalisation par impression client.</summary>
    public required bool IsCustomizable { get; init; }

    /// <summary>Appartient à la gamme gastronomique/traiteur premium de Phoenix.</summary>
    public required bool IsGourmetRange { get; init; }

    /// <summary>Vendu uniquement en grande quantité (conditionnement vrac industriel).</summary>
    public required bool IsBulkOnly { get; init; }

    /// <summary>Certifié éco-responsable (recyclable, compostable, biosourcé…).</summary>
    public required bool IsEcoFriendly { get; init; }

    /// <summary>Certifié contact alimentaire selon la réglementation CE 1935/2004.</summary>
    public required bool IsFoodApproved { get; init; }

    /// <summary>Vendu au poids (kg) plutôt qu'à l'unité.</summary>
    public required bool SoldByWeight { get; init; }

    /// <summary>Disponible en livraison express J+1 depuis le stock Phoenix.</summary>
    public required bool HasExpressDelivery { get; init; }
}
