namespace Phoenix.Domain.Products.Events;

/// <summary>
/// Événement de domaine publié lors de la mise à jour d'un produit existant
/// (libellé, description, flags, variantes, paliers tarifaires ou images).
/// </summary>
/// <remarks>
/// Abonnés typiques :
/// <list type="bullet">
///   <item>Re-indexation dans le moteur de recherche.</item>
///   <item>Invalidation du cache de la fiche produit.</item>
///   <item>Notification des clients ayant ce produit en favori.</item>
/// </list>
/// </remarks>
/// <param name="ProductId">Identifiant de l'agrégat Product mis à jour.</param>
/// <param name="OccurredAtUtc">Horodatage UTC de l'événement.</param>
public sealed record ProductUpdatedEvent(Guid ProductId, DateTime OccurredAtUtc);
