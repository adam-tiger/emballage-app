using Phoenix.Domain.Common.Primitives;

namespace Phoenix.Domain.Catalog.Events;

/// <summary>
/// Événement publié lorsqu'un produit est désactivé (retiré du catalogue public).
/// </summary>
/// <remarks>
/// Abonnés typiques :
/// <list type="bullet">
///   <item>Module Search : désindexer ou marquer le produit comme inactif.</item>
///   <item>Module Order : vérifier les lignes de commandes en cours sur ce produit.</item>
///   <item>Module Cart : notifier les paniers contenant ce produit.</item>
///   <item>Module Notification : alerter les commerciaux responsables du produit.</item>
/// </list>
/// </remarks>
/// <param name="ProductId">Identifiant de l'agrégat Product désactivé.</param>
/// <param name="Reference">Référence SKU du produit (immuable, fourni pour faciliter le routing).</param>
public sealed record ProductDeactivatedEvent(
    Guid ProductId,
    string Reference) : IDomainEvent
{
    /// <inheritdoc/>
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc/>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
