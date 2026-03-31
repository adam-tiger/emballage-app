using Phoenix.Domain.Common.Primitives;

namespace Phoenix.Domain.Catalog.Events;

/// <summary>
/// Événement publié lors de la mise à jour d'un produit existant (libellé, description,
/// segment cible, flags, paliers tarifaires, variantes ou images).
/// </summary>
/// <remarks>
/// Abonnés typiques :
/// <list type="bullet">
///   <item>Module Search : re-indexer le produit modifié.</item>
///   <item>Module Cache : invalider le cache de la fiche produit.</item>
///   <item>Module Notification : notifier les clients ayant ce produit en favori.</item>
/// </list>
/// </remarks>
/// <param name="ProductId">Identifiant de l'agrégat Product mis à jour.</param>
/// <param name="Reference">Référence SKU du produit (immuable, fourni pour faciliter le routing).</param>
public sealed record ProductUpdatedEvent(
    Guid ProductId,
    string Reference) : IDomainEvent
{
    /// <inheritdoc/>
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc/>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
