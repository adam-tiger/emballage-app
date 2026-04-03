using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core de l'entité <see cref="ProductVariant"/>.
/// Table PostgreSQL : <c>product_variants</c>.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>PK : <c>Id</c> — UUIDv7, <c>ValueGeneratedNever</c>.</item>
///   <item>Index unique sur <c>Sku</c> (unicité catalogue globale).</item>
///   <item><c>PrintSide</c> et <c>ColorCount</c> : enums stockés en string via <c>HasConversion&lt;string&gt;()</c>.</item>
///   <item>Navigation <c>PriceTiers</c> configurée avec le backing field privé <c>_priceTiers</c>.</item>
/// </list>
/// </remarks>
internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");

        // ── Clé primaire ──────────────────────────────────────────────────
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();

        // ── Clé étrangère ─────────────────────────────────────────────────
        builder.Property(v => v.ProductId).IsRequired();

        // ── Propriétés scalaires ──────────────────────────────────────────
        builder.Property(v => v.Sku)
            .IsRequired()
            .HasMaxLength(80);

        builder.HasIndex(v => v.Sku).IsUnique();

        builder.Property(v => v.NameFr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.MinimumOrderQuantity).IsRequired();

        builder.Property(v => v.PrintSide)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(v => v.ColorCount)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(v => v.CreatedAtUtc).IsRequired();

        // ── Paliers tarifaires : backing field _priceTiers ────────────────
        builder.HasMany(v => v.PriceTiers)
            .WithOne()
            .HasForeignKey(t => t.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.PriceTiers)
            .HasField("_priceTiers")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
