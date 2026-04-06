using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Entities;
using Phoenix.Domain.Products.Entities;
using Phoenix.Infrastructure.Identity;
using Phoenix.Infrastructure.Persistence.DataSeed;

namespace Phoenix.Infrastructure.Persistence;

/// <summary>
/// Contexte EF Core principal de l'application Phoenix.
/// Hérite de <see cref="IdentityDbContext{TUser,TRole,TKey}"/> pour intégrer ASP.NET Identity
/// (tables <c>AspNetUsers</c>, <c>AspNetRoles</c>, etc., renommées dans les configurations).
/// Dispatch les événements de domaine via MediatR après chaque <see cref="SaveChangesAsync"/>.
/// </summary>
public sealed class PhoenixDbContext(
    DbContextOptions<PhoenixDbContext> options,
    IMediator mediator)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    // ── DbSets — Products ────────────────────────────────────────────────────

    /// <summary>Agrégats <see cref="Product"/> — table <c>products</c>.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>Variantes d'impression — table <c>product_variants</c>.</summary>
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    /// <summary>Paliers tarifaires — table <c>price_tiers</c>.</summary>
    public DbSet<PriceTier> PriceTiers => Set<PriceTier>();

    /// <summary>Images produit — table <c>product_images</c>.</summary>
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    // ── DbSets — Customers ───────────────────────────────────────────────────

    /// <summary>Agrégats <see cref="Customer"/> — table <c>customers</c>.</summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>Adresses de livraison — table <c>customer_addresses</c>.</summary>
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

    // ── DbSets — Identity ────────────────────────────────────────────────────

    /// <summary>Refresh tokens persistés — table <c>refresh_tokens</c>.</summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // ── Model configuration ──────────────────────────────────────────────────

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applique toutes les IEntityTypeConfiguration<T> de l'assemblée
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PhoenixDbContext).Assembly);

        // Données de référence initiales
        ProductDataSeed.Seed(modelBuilder);
    }

    // ── SaveChangesAsync ─────────────────────────────────────────────────────

    /// <summary>
    /// Persiste les modifications, met à jour <see cref="Product.UpdatedAtUtc"/> et
    /// <see cref="Customer.UpdatedAtUtc"/> pour chaque agrégat modifié, puis dispatche
    /// les événements de domaine accumulés via MediatR.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // ── 1. Auto-mise à jour de UpdatedAtUtc ──────────────────────────────
        foreach (var entry in ChangeTracker.Entries<Product>())
        {
            if (entry.State is EntityState.Modified or EntityState.Added)
                entry.Property(nameof(Product.UpdatedAtUtc)).CurrentValue = DateTime.UtcNow;
        }

        // ── 2. Collecte des événements de domaine (Products + Customers) ─────
        var productsWithEvents = ChangeTracker
            .Entries<Product>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var customersWithEvents = ChangeTracker
            .Entries<Customer>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = productsWithEvents
            .SelectMany(p => p.DomainEvents)
            .Concat(customersWithEvents.SelectMany(c => c.DomainEvents))
            .ToList();

        foreach (var p in productsWithEvents)  p.ClearDomainEvents();
        foreach (var c in customersWithEvents) c.ClearDomainEvents();

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
