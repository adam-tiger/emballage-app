using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Domain.Customizations.Events;

/// <summary>
/// Événement de domaine émis lorsqu'un <c>CustomizationJob</c> est finalisé
/// via <c>CustomizationJob.Finalize()</c>.
/// Le job passe au statut <c>ReadyForOrder</c> et peut être référencé
/// dans une <c>OrderLine</c> au Module 5.
/// </summary>
/// <param name="JobId">Identifiant unique du job finalisé.</param>
/// <param name="CustomerId">
/// Identifiant du client propriétaire.
/// <c>null</c> si le configurateur a été utilisé en mode invité.
/// </param>
/// <param name="ProductVariantId">Identifiant de la variante produit personnalisée.</param>
/// <param name="PrintSide">Face(s) d'impression retenue(s) pour ce job.</param>
/// <param name="ColorCount">Nombre de couleurs d'impression retenu pour ce job.</param>
/// <param name="OccurredAtUtc">Horodatage UTC de l'événement.</param>
public record CustomizationFinalizedEvent(
    Guid        JobId,
    Guid?       CustomerId,
    Guid        ProductVariantId,
    PrintSide   PrintSide,
    ColorCount  ColorCount,
    DateTime    OccurredAtUtc);
