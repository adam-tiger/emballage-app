using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phoenix.Infrastructure.Identity;

namespace Phoenix.Infrastructure.Identity.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité <see cref="RefreshToken"/>.
/// </summary>
public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .HasMaxLength(256)
            .IsRequired();

        // Index unique pour les recherches rapides par valeur de token
        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.Property(rt => rt.UserId)
            .HasMaxLength(450)
            .IsRequired();

        // Index pour la révocation massive par utilisateur
        builder.HasIndex(rt => rt.UserId);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.IsRevoked)
            .HasDefaultValue(false);

        builder.Property(rt => rt.CreatedAt)
            .IsRequired();
    }
}
