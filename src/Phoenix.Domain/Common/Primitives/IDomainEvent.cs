namespace Phoenix.Domain.Common.Primitives;

/// <summary>
/// Marque une classe comme événement du domaine.
/// Les événements de domaine sont publiés après validation des invariants métier,
/// via un dispatcher (MediatR INotification) dans la couche Infrastructure.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Identifiant unique de l'occurrence de l'événement.</summary>
    Guid Id { get; }

    /// <summary>Horodatage UTC de l'occurrence.</summary>
    DateTime OccurredOn { get; }
}
