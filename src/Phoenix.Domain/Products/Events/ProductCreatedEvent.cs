namespace Phoenix.Domain.Products.Events;

/// <summary>
/// Événement de domaine publié lors de la création d'un nouveau produit.
/// </summary>
/// <remarks>
/// Abonnés typiques (via MediatR INotificationHandler dans Phoenix.Application) :
/// <list type="bullet">
///   <item>Indexation dans le moteur de recherche.</item>
///   <item>Notification commerciale d'un nouveau produit catalogue.</item>
/// </list>
/// </remarks>
/// <param name="ProductId">Identifiant de l'agrégat Product nouvellement créé.</param>
/// <param name="OccurredAtUtc">Horodatage UTC de l'événement.</param>
public sealed record ProductCreatedEvent(Guid ProductId, DateTime OccurredAtUtc);
