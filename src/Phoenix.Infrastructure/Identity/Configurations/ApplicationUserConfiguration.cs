using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phoenix.Application.Common.Identity;

namespace Phoenix.Infrastructure.Identity.Configurations;

/// <summary>
/// Configuration EF Core pour <see cref="ApplicationUser"/>.
/// Complète la configuration Identity générée automatiquement par <c>IdentityDbContext</c>.
/// </summary>
public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("application_users");

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.CompanyName)
            .HasMaxLength(200);

        builder.Property(u => u.Segment)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAtUtc)
            .IsRequired();

        builder.Property(u => u.CustomerId);

        // Propriété calculée — non mappée en colonne
        builder.Ignore(u => u.FullName);
    }
}
