namespace Phoenix.Domain.Customers.Events;

/// <summary>
/// Événement de domaine émis lorsqu'un client ajoute une nouvelle adresse de livraison
/// à son profil. Peut être utilisé pour déclencher une notification de confirmation.
/// </summary>
/// <param name="CustomerId">Identifiant unique du client.</param>
/// <param name="AddressId">Identifiant de l'adresse nouvellement ajoutée.</param>
/// <param name="OccurredAtUtc">Horodatage UTC de l'ajout.</param>
public record CustomerAddressAddedEvent(
    Guid     CustomerId,
    Guid     AddressId,
    DateTime OccurredAtUtc);
