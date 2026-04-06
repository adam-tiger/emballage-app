using Phoenix.Domain.Customizations.Entities;
using Phoenix.Domain.Customizations.ValueObjects;

namespace Phoenix.Domain.Customizations.Repositories;

/// <summary>
/// Port (interface) du repository pour l'agrégat <see cref="CustomizationJob"/>.
/// Implémenté dans la couche Infrastructure (EF Core + PostgreSQL).
/// </summary>
public interface ICustomizationRepository
{
    /// <summary>
    /// Récupère un job de personnalisation par son identifiant unique.
    /// </summary>
    /// <param name="id">Identifiant unique du <see cref="CustomizationJob"/>.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>Le <see cref="CustomizationJob"/> trouvé, ou <c>null</c> s'il n'existe pas.</returns>
    Task<CustomizationJob?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Récupère tous les jobs de personnalisation d'un client donné,
    /// triés par date de création décroissante.
    /// </summary>
    /// <param name="customerId">Identifiant du client propriétaire des jobs.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>
    /// Liste immuable des <see cref="CustomizationJob"/> du client,
    /// ou une liste vide si aucun job n'est trouvé.
    /// </returns>
    Task<IReadOnlyList<CustomizationJob>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct = default);

    /// <summary>
    /// Persiste un nouveau job de personnalisation en base de données.
    /// </summary>
    /// <param name="job">Instance de <see cref="CustomizationJob"/> à ajouter.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task AddAsync(CustomizationJob job, CancellationToken ct = default);

    /// <summary>
    /// Marque un job existant comme modifié pour que EF Core le persiste
    /// lors du prochain <c>SaveChangesAsync</c>.
    /// </summary>
    /// <param name="job">Instance de <see cref="CustomizationJob"/> à mettre à jour.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task UpdateAsync(CustomizationJob job, CancellationToken ct = default);

    /// <summary>
    /// Récupère le dernier job actif (statut <see cref="CustomizationStatus.Draft"/>
    /// ou <see cref="CustomizationStatus.LogoUploaded"/>) pour un client et une variante
    /// produit donnés.
    /// Permet de reprendre une session de configuration interrompue.
    /// </summary>
    /// <param name="customerId">Identifiant du client propriétaire.</param>
    /// <param name="variantId">Identifiant de la variante produit ciblée.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>
    /// Le <see cref="CustomizationJob"/> en cours (Draft ou LogoUploaded) le plus récent,
    /// ou <c>null</c> si aucun job actif n'existe pour cette combinaison client/variante.
    /// </returns>
    Task<CustomizationJob?> GetActiveJobForVariantAsync(
        Guid customerId,
        Guid variantId,
        CancellationToken ct = default);
}
