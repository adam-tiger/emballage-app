using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core de l'agrégat racine <see cref="Product"/>.
/// Table PostgreSQL : <c>products</c>.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>PK : <c>Id</c> — UUIDv7, <c>ValueGeneratedNever</c>.</item>
///   <item>Index unique sur <c>Sku</c>.</item>
///   <item><c>Family</c> : enum stocké en string via <c>HasConversion&lt;string&gt;()</c>.</item>
///   <item>Navigations <c>Variants</c> et <c>Images</c> configurées avec leur backing field privé.</item>
///   <item><c>DomainEvents</c> ignoré — non persisté en base.</item>
/// </list>
/// </remarks>
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        // ── Clé primaire ──────────────────────────────────────────────────
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        // ── Propriétés scalaires ──────────────────────────────────────────
        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Sku).IsUnique();

        builder.Property(p => p.NameFr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.DescriptionFr)
            .HasMaxLength(2000)
            .HasDefaultValue(string.Empty);

        builder.Property(p => p.Family)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (ProductFamily)Enum.Parse(typeof(ProductFamily), v))
            .HasMaxLength(50);

        builder.Property(p => p.IsCustomizable).IsRequired();
        builder.Property(p => p.IsGourmetRange).IsRequired();
        builder.Property(p => p.IsBulkOnly).IsRequired();
        builder.Property(p => p.IsEcoFriendly).IsRequired();
        builder.Property(p => p.IsFoodApproved).IsRequired();
        builder.Property(p => p.SoldByWeight).IsRequired();
        builder.Property(p => p.HasExpressDelivery).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();

        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();

        // ── Relations (eager loading explicite — pas de lazy loading) ─────

        // Variantes d'impression : backing field _variants
        builder.HasMany(p => p.Variants)
            .WithOne()
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Variants)
            .HasField("_variants")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Images : backing field _images
        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Images)
            .HasField("_images")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ── Ignoré ────────────────────────────────────────────────────────
        // DomainEvents n'est pas une navigation EF Core — ignoré explicitement.
        builder.Ignore(p => p.DomainEvents);
    }
}
