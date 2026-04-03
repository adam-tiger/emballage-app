using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core de l'entité <see cref="ProductImage"/>.
/// Table PostgreSQL : <c>product_images</c>.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>PK : <c>Id</c> — UUIDv7, <c>ValueGeneratedNever</c>.</item>
///   <item><c>BlobPath</c> : chemin RELATIF dans le conteneur blob (max 500 chars).</item>
///   <item><c>PublicUrl</c> : URL CDN complète (max 1000 chars).</item>
///   <item>Colonnes thumb optionnelles : <c>ThumbBlobPath</c>, <c>ThumbPublicUrl</c>.</item>
/// </list>
/// </remarks>
internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");

        // ── Clé primaire ──────────────────────────────────────────────────
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        // ── Clé étrangère ─────────────────────────────────────────────────
        builder.Property(i => i.ProductId).IsRequired();

        // ── Propriétés scalaires ──────────────────────────────────────────
        builder.Property(i => i.BlobPath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.PublicUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(i => i.ThumbBlobPath)
            .HasMaxLength(500);

        builder.Property(i => i.ThumbPublicUrl)
            .HasMaxLength(1000);

        builder.Property(i => i.IsMain).IsRequired();

        builder.Property(i => i.CreatedAtUtc).IsRequired();
    }
}
