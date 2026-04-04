using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core de l'entité <see cref="PriceTier"/>.
/// Table PostgreSQL : <c>price_tiers</c>.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>PK : <c>Id</c> — UUIDv7, <c>ValueGeneratedNever</c>.</item>
///   <item><c>UnitPriceHT</c> : <c>decimal(18, 4)</c> — jamais float ou double.</item>
/// </list>
/// </remarks>
internal sealed class PriceTierConfiguration : IEntityTypeConfiguration<PriceTier>
{
    public void Configure(EntityTypeBuilder<PriceTier> builder)
    {
        builder.ToTable("price_tiers");

        // ── Clé primaire ──────────────────────────────────────────────────
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        // ── Clé étrangère ─────────────────────────────────────────────────
        builder.Property(t => t.ProductVariantId).IsRequired();

        // ── Propriétés scalaires ──────────────────────────────────────────
        builder.Property(t => t.MinQuantity).IsRequired();

        builder.Property(t => t.MaxQuantity);  // nullable — sans plafond si null

        builder.Property(t => t.UnitPriceHT)
            .IsRequired()
            .HasPrecision(18, 4);
    }
}
