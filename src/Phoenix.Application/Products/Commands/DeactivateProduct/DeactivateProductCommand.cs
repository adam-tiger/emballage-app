using MediatR;

namespace Phoenix.Application.Products.Commands.DeactivateProduct;

/// <summary>
/// Commande de désactivation (soft-delete) d'un produit qui le retire du catalogue public.
/// </summary>
public sealed record DeactivateProductCommand : IRequest<Unit>
{
    /// <summary>Identifiant du produit à désactiver.</summary>
    public required Guid Id { get; init; }
}
