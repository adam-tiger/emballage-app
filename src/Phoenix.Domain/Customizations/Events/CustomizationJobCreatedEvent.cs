namespace Phoenix.Domain.Customizations.Events;

/// <summary>
/// Événement de domaine émis à la création d'un nouveau <c>CustomizationJob</c>.
/// Permet de déclencher des effets de bord (analytics, notification, réservation temporaire)
/// sans coupler le domaine à l'infrastructure.
/// </summary>
/// <param name="JobId">Identifiant unique du job de personnalisation créé.</param>
/// <param name="ProductId">Identifiant du produit du catalogue associé.</param>
/// <param name="ProductVariantId">Identifiant de la variante produit sélectionnée.</param>
/// <param name="CustomerId">
/// Identifiant du client propriétaire du job.
/// <c>null</c> si le configurateur est utilisé en mode invité (sans compte).
/// </param>
/// <param name="OccurredAtUtc">Horodatage UTC de l'événement.</param>
public record CustomizationJobCreatedEvent(
    Guid      JobId,
    Guid      ProductId,
    Guid      ProductVariantId,
    Guid?     CustomerId,
    DateTime  OccurredAtUtc);
