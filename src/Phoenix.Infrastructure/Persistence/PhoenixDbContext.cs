using MediatR;
using Microsoft.EntityFrameworkCore;
using Phoenix.Domain.Products.Entities;
using Phoenix.Infrastructure.Persistence.DataSeed;

namespace Phoenix.Infrastructure.Persistence;

/// <summary>
/// Contexte EF Core principal de l'application Phoenix.
/// Dispatch les événements de domaine via MediatR après chaque <see cref="SaveChangesAsync"/>.
/// </summary>
/// <remarks>
/// Configure les entités du module <c>Products</c> via <c>ApplyConfigurationsFromAssembly</c>.
/// Les données de référence sont initialisées par <see cref="ProductDataSeed"/>.
/// </remarks>
public sealed class PhoenixDbContext(
    DbContextOptions<PhoenixDbContext> options,
    IMediator mediator) : DbContext(options)
{
    // ── DbSets ──────────────────────────────────────────────────────────────

    /// <summary>Agrégats <see cref="Product"/> — table <c>products</c>.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>Variantes d'impression — table <c>product_variants</c>.</summary>
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    /// <summary>Paliers tarifaires — table <c>price_tiers</c>.</summary>
    public DbSet<PriceTier> PriceTiers => Set<PriceTier>();

    /// <summary>Images produit — table <c>product_images</c>.</summary>
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    // ── Model configuration ─────────────────────────────────────────────────

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applique toutes les IEntityTypeConfiguration<T> de l'assemblée (sans réflexion à l'exécution)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PhoenixDbContext).Assembly);

        // Données de référence initiales
        ProductDataSeed.Seed(modelBuilder);
    }

    // ── SaveChangesAsync ────────────────────────────────────────────────────

    /// <summary>
    /// Persiste les modifications, met à jour <see cref="Product.UpdatedAtUtc"/> pour chaque
    /// agrégat modifié, puis dispatche les événements de domaine accumulés via MediatR.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Nombre de lignes affectées.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // ── 1. Auto-mise à jour de UpdatedAtUtc ──────────────────────────────
        foreach (var entry in ChangeTracker.Entries<Product>())
        {
            if (entry.State is EntityState.Modified or EntityState.Added)
                entry.Property(nameof(Product.UpdatedAtUtc)).CurrentValue = DateTime.UtcNow;
        }

        // ── 2. Collecte et purge des événements de domaine ───────────────────
        var aggregatesWithEvents = ChangeTracker
            .Entries<Product>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregatesWithEvents
            .SelectMany(p => p.DomainEvents)
            .ToList();

        foreach (var aggregate in aggregatesWithEvents)
            aggregate.ClearDomainEvents();

        // ── 3. Persistance ───────────────────────────────────────────────────
        var result = await base.SaveChangesAsync(cancellationToken);

        // ── 4. Dispatch des événements après commit réussi ───────────────────
        foreach (var domainEvent in domainEvents)
        {
            if (domainEvent is INotification notification)
                await mediator.Publish(notification, cancellationToken);
        }

        return result;
    }
}
