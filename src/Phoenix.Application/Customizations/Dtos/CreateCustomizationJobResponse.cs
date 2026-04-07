namespace Phoenix.Application.Customizations.Dtos;

/// <summary>
/// Réponse retournée après la création (ou la récupération) d'un job de personnalisation.
/// Contient les informations minimales pour démarrer la session de configuration côté Angular.
/// </summary>
/// <param name="JobId">Identifiant unique du job créé ou récupéré.</param>
/// <param name="ProductId">Identifiant du produit du catalogue associé.</param>
/// <param name="ProductVariantId">Identifiant de la variante produit sélectionnée.</param>
/// <param name="CustomerId">
/// Identifiant du client propriétaire.
/// <c>null</c> si le configurateur est utilisé en mode invité.
/// </param>
/// <param name="Status">Statut initial du job (toujours <c>"Draft"</c> à la création).</param>
/// <param name="PrintCoefficient">Coefficient d'impression initial (1.00 — SingleSide + 1 couleur).</param>
/// <param name="PrintSide">Option de face d'impression initiale (toujours <c>"SingleSide"</c>).</param>
/// <param name="ColorCount">Nombre de couleurs initial (toujours <c>"One"</c>).</param>
public sealed record CreateCustomizationJobResponse(
    Guid    JobId,
    Guid    ProductId,
    Guid    ProductVariantId,
    Guid?   CustomerId,
    string  Status,
    decimal PrintCoefficient,
    string  PrintSide,
    string  ColorCount);
