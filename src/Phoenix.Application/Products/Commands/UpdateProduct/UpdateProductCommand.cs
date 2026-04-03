using MediatR;

namespace Phoenix.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Commande de mise à jour du libellé et de la description d'un produit existant.
/// </summary>
public sealed record UpdateProductCommand : IRequest<Unit>
{
    /// <summary>Identifiant du produit à mettre à jour.</summary>
    public required Guid Id { get; init; }

    /// <summary>Nouveau libellé commercial en français.</summary>
    public required string NameFr { get; init; }

    /// <summary>Nouvelle description longue en français.</summary>
    public required string DescriptionFr { get; init; }
}
