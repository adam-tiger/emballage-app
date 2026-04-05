namespace Phoenix.Domain.Customers.Events;

/// <summary>
/// Événement de domaine émis lorsqu'un nouveau client s'inscrit sur Phoenix Emballages.
/// Utilisé pour déclencher l'envoi de l'e-mail de bienvenue et initialiser
/// les données de suivi analytics.
/// </summary>
/// <param name="CustomerId">Identifiant unique du client nouvellement créé.</param>
/// <param name="Email">Adresse e-mail du client (destinataire du mail de bienvenue).</param>
/// <param name="FullName">Nom complet du client (prénom + nom).</param>
/// <param name="OccurredAtUtc">Horodatage UTC de l'événement.</param>
public record CustomerRegisteredEvent(
    Guid     CustomerId,
    string   Email,
    string   FullName,
    DateTime OccurredAtUtc);
