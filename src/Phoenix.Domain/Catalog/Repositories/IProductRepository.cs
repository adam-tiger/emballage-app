using Phoenix.Domain.Catalog.Aggregates;
using Phoenix.Domain.Catalog.ValueObjects;

namespace Phoenix.Domain.Catalog.Repositories;

/// <summary>
/// Port (interface) du repository produit.
/// L'implémentation concrète réside dans Phoenix.Infrastructure (EF Core + PostgreSQL).
/// </summary>
/// <remarks>
/// Ce repository suit le pattern Repository du DDD : il travaille exclusivement
/// avec l'agrégat <see cref="Product"/> et non avec des entités enfants isolées.
/// Toute opération en base doit passer par cet agrégat pour garantir les invariants.
/// </remarks>
public interface IProductRepository
{
    // ----- Lectures -----

    /// <summary>
    /// Retourne un produit par son identifiant, avec toutes ses collections chargées
    /// (variantes, paliers, images).
    /// </summary>
    /// <returns><c>null</c> si le produit n'existe pas.</returns>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne un produit par sa référence SKU (insensible à la casse).
    /// </summary>
    /// <returns><c>null</c> si le produit n'existe pas.</returns>
    Task<Product?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne tous les produits d'une famille donnée.
    /// </summary>
    Task<IReadOnlyList<Product>> GetByFamilyAsync(
        ProductFamily family,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne tous les produits actifs du catalogue (IsActive = true).
    /// </summary>
    Task<IReadOnlyList<Product>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne les produits actifs d'un segment client donné.
    /// </summary>
    Task<IReadOnlyList<Product>> GetActiveBySegmentAsync(
        CustomerSegment segment,
        CancellationToken cancellationToken = default);

    // ----- Vérifications d'existence -----

    /// <summary>
    /// Vérifie si un produit existe avec cet identifiant.
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si une référence SKU est déjà utilisée dans le catalogue.
    /// Utile pour valider l'unicité avant création.
    /// </summary>
    Task<bool> ReferenceExistsAsync(string reference, CancellationToken cancellationToken = default);

    // ----- Écritures -----

    /// <summary>
    /// Persiste un nouveau produit.
    /// L'unité de travail (UnitOfWork / DbContext.SaveChangesAsync) doit être appelée
    /// séparément par le handler Application.
    /// </summary>
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signale à l'UnitOfWork qu'un produit existant a été modifié (EF Core tracking).
    /// Dans la plupart des implémentations EF Core, cet appel est optionnel si l'entité
    /// est déjà trackée, mais il est fourni pour les scénarios de déconnexion.
    /// </summary>
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}
