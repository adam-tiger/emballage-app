using Phoenix.Domain.Catalog.ValueObjects;
using Phoenix.Domain.Common.Primitives;

namespace Phoenix.Domain.Catalog.Events;

/// <summary>
/// Événement publié lors de la création d'un nouveau produit dans le catalogue.
/// </summary>
/// <remarks>
/// Abonnés typiques :
/// <list type="bullet">
///   <item>Module Search : indexer le produit dans le moteur de recherche.</item>
///   <item>Module Notification : alerter les commerciaux d'un nouveau produit.</item>
/// </list>
/// </remarks>
/// <param name="ProductId">Identifiant de l'agrégat Product nouvellement créé.</param>
/// <param name="Reference">Référence SKU du produit.</param>
/// <param name="Family">Famille de produits du catalogue Phoenix.</param>
public sealed record ProductCreatedEvent(
    Guid ProductId,
    string Reference,
    ProductFamily Family) : IDomainEvent
{
    /// <inheritdoc/>
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc/>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
