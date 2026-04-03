using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Domain.Common.Interfaces;

// ── Filtres et résultats paginés ─────────────────────────────────────────────

/// <summary>
/// Critères de filtrage et de pagination pour la liste des produits.
/// Tous les filtres sont optionnels et combinables.
/// </summary>
public sealed record ProductListFilter
{
    /// <summary>Numéro de page (base 1). Défaut : 1.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Nombre d'éléments par page. Défaut : 20.</summary>
    public int PageSize { get; init; } = 20;

    /// <summary>Propriété de tri (ex : "sku", "nameFr", "createdAtUtc"). Null = tri par défaut.</summary>
    public string? SortBy { get; init; }

    /// <summary>Direction de tri : "asc" ou "desc". Null = "asc".</summary>
    public string? SortDir { get; init; }

    /// <summary>Filtre sur la famille de produits.</summary>
    public ProductFamily? Family { get; init; }

    /// <summary>Filtre sur le segment client cible.</summary>
    public CustomerSegment? Segment { get; init; }

    /// <summary>Filtre sur les produits personnalisables uniquement.</summary>
    public bool? IsCustomizable { get; init; }

    /// <summary>Recherche plein-texte sur le SKU, NameFr et DescriptionFr.</summary>
    public string? SearchText { get; init; }

    /// <summary>Filtre sur l'état actif / inactif du produit. Null = tous.</summary>
    public bool? IsActive { get; init; }
}

/// <summary>
/// Résultat paginé générique retourné par les queries de liste.
/// </summary>
/// <typeparam name="T">Type des éléments de la page.</typeparam>
/// <param name="Items">Éléments de la page courante.</param>
/// <param name="Page">Numéro de la page courante (base 1).</param>
/// <param name="PageSize">Taille de la page.</param>
/// <param name="TotalCount">Nombre total d'éléments toutes pages confondues.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    /// <summary>Nombre total de pages calculé depuis <see cref="TotalCount"/> et <see cref="PageSize"/>.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}

// ── Interface ────────────────────────────────────────────────────────────────

/// <summary>
/// Port (interface) du repository produit.
/// L'implémentation concrète réside dans <c>Phoenix.Infrastructure</c> (EF Core + PostgreSQL).
/// Ce repository travaille exclusivement avec l'agrégat <see cref="Product"/>.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retourne un produit par son identifiant avec toutes ses collections chargées
    /// (variantes, paliers tarifaires, images).
    /// </summary>
    /// <param name="id">Identifiant du produit.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns><see cref="Product"/> ou <c>null</c> si non trouvé.</returns>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Retourne un produit par son SKU (insensible à la casse).
    /// </summary>
    /// <param name="sku">Référence SKU du produit.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns><see cref="Product"/> ou <c>null</c> si non trouvé.</returns>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);

    /// <summary>
    /// Retourne une page de produits selon les critères de <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">Filtres de liste, pagination et tri.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>Page de résultats.</returns>
    Task<PagedResult<Product>> GetListAsync(ProductListFilter filter, CancellationToken ct = default);

    /// <summary>
    /// Persiste un nouveau produit.
    /// <c>IUnitOfWork.SaveChangesAsync</c> doit être appelé séparément par le handler.
    /// </summary>
    /// <param name="product">Agrégat à persister.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task AddAsync(Product product, CancellationToken ct = default);

    /// <summary>
    /// Signale une modification sur un produit existant (EF Core change tracking).
    /// Dans la plupart des cas, cet appel est implicite si l'entité est trackée.
    /// </summary>
    /// <param name="product">Agrégat modifié.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task UpdateAsync(Product product, CancellationToken ct = default);

    /// <summary>
    /// Vérifie si un SKU est déjà utilisé dans le catalogue.
    /// </summary>
    /// <param name="sku">SKU à tester.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns><c>true</c> si le SKU existe déjà.</returns>
    Task<bool> ExistsAsync(string sku, CancellationToken ct = default);
}
