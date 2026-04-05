namespace Phoenix.Domain.Customers.Events;

/// <summary>
/// Événement de domaine émis lorsqu'un client met à jour son profil
/// (prénom, nom, raison sociale ou segment professionnel).
/// Peut être utilisé pour synchroniser des systèmes tiers (CRM, analytics).
/// </summary>
/// <param name="CustomerId">Identifiant unique du client dont le profil a été modifié.</param>
/// <param name="OccurredAtUtc">Horodatage UTC de la mise à jour.</param>
public record CustomerProfileUpdatedEvent(
    Guid     CustomerId,
    DateTime OccurredAtUtc);
