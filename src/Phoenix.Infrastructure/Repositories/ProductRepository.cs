using Microsoft.EntityFrameworkCore;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;
using Phoenix.Infrastructure.Persistence;

namespace Phoenix.Infrastructure.Repositories;

/// <summary>
/// Implémentation EF Core de <see cref="IProductRepository"/>.
/// Toutes les requêtes utilisent le <b>eager loading explicite</b> via <c>Include / ThenInclude</c>
/// — aucun lazy loading configuré.
/// </summary>
internal sealed class ProductRepository(PhoenixDbContext context) : IProductRepository
{
    // ── Requête de base avec eager loading ───────────────────────────────────

    /// <summary>
    /// Requête de base avec chargement complet des collections.
    /// Variantes → Paliers tarifaires + Images.
    /// </summary>
    private IQueryable<Product> FullQuery =>
        context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.PriceTiers)
            .Include(p => p.Images);

    // ── IProductRepository ──────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await FullQuery
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    /// <inheritdoc />
    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default) =>
        await FullQuery
            .FirstOrDefaultAsync(p => p.Sku == sku.Trim().ToUpperInvariant(), ct);

    /// <inheritdoc />
    public async Task<PagedResult<Product>> GetListAsync(
        ProductListFilter filter,
        CancellationToken ct = default)
    {
        var query = context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.PriceTiers)
            .Include(p => p.Images)
            .AsQueryable();

        // ── Filtres dynamiques ───────────────────────────────────────────────

        if (filter.Family.HasValue)
            query = query.Where(p => p.Family == filter.Family.Value);

        if (filter.IsCustomizable.HasValue)
            query = query.Where(p => p.IsCustomizable == filter.IsCustomizable.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(p => p.IsActive == filter.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            // ILike = recherche insensible à la casse sur PostgreSQL (extension Npgsql)
            var pattern = $"%{EscapeLikePattern(filter.SearchText)}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Sku, pattern) ||
                EF.Functions.ILike(p.NameFr, pattern) ||
                EF.Functions.ILike(p.DescriptionFr, pattern));
        }

        // ── Comptage total avant pagination ──────────────────────────────────
        var totalCount = await query.CountAsync(ct);

        // ── Tri dynamique ────────────────────────────────────────────────────
        var descending = string.Equals(filter.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

        query = filter.SortBy?.ToLowerInvariant() switch
        {
            "sku"          => descending ? query.OrderByDescending(p => p.Sku)         : query.OrderBy(p => p.Sku),
            "namefr"       => descending ? query.OrderByDescending(p => p.NameFr)      : query.OrderBy(p => p.NameFr),
            "createdat"
            or "createdatutc"
                           => descending ? query.OrderByDescending(p => p.CreatedAtUtc) : query.OrderBy(p => p.CreatedAtUtc),
            "updatedat"
            or "updatedatutc"
                           => descending ? query.OrderByDescending(p => p.UpdatedAtUtc) : query.OrderBy(p => p.UpdatedAtUtc),
            _              => descending ? query.OrderByDescending(p => p.CreatedAtUtc) : query.OrderBy(p => p.CreatedAtUtc)
        };

        // ── Pagination ───────────────────────────────────────────────────────
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Product>(items, filter.Page, filter.PageSize, totalCount);
    }

    /// <inheritdoc />
    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await context.Products.AddAsync(product, ct);

    /// <inheritdoc />
    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        // EF Core tracke automatiquement les entités chargées via GetByIdAsync.
        // Cet appel explicite garantit le tracking si le produit a été détaché.
        context.Products.Update(product);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string sku, CancellationToken ct = default) =>
        await context.Products
            .AnyAsync(p => p.Sku == sku.Trim().ToUpperInvariant(), ct);

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Échappe les caractères spéciaux de pattern LIKE pour PostgreSQL (<c>%</c>, <c>_</c>, <c>\</c>).
    /// </summary>
    private static string EscapeLikePattern(string input) =>
        input.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
