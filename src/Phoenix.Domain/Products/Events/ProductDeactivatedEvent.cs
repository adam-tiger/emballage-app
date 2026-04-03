namespace Phoenix.Domain.Products.Events;

/// <summary>
/// Événement de domaine publié lorsqu'un produit est désactivé (retiré du catalogue public).
/// </summary>
/// <remarks>
/// Abonnés typiques :
/// <list type="bullet">
///   <item>Désindexation ou marquage inactif dans le moteur de recherche.</item>
///   <item>Vérification des lignes de commandes en cours sur ce produit.</item>
///   <item>Notification des paniers clients contenant ce produit.</item>
///   <item>Alerte commerciale interne.</item>
/// </list>
/// </remarks>
/// <param name="ProductId">Identifiant de l'agrégat Product désactivé.</param>
/// <param name="OccurredAtUtc">Horodatage UTC de l'événement.</param>
public sealed record ProductDeactivatedEvent(Guid ProductId, DateTime OccurredAtUtc);
