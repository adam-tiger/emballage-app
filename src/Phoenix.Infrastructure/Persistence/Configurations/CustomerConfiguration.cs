using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phoenix.Domain.Customers.Entities;

namespace Phoenix.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'agrégat racine <see cref="Customer"/>.
/// </summary>
public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.ApplicationUserId)
            .IsRequired();

        builder.HasIndex(c => c.ApplicationUserId)
            .IsUnique();

        builder.Property(c => c.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.CompanyName)
            .HasMaxLength(200);

        builder.Property(c => c.Segment)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired();

        // Propriété calculée — non mappée
        builder.Ignore(c => c.FullName);

        // Collections d'événements de domaine — non persistées
        builder.Ignore(c => c.DomainEvents);

        // Navigation vers les adresses
        builder.HasMany(c => c.Addresses)
            .WithOne()
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Accès aux backing fields privés
        builder.Navigation(c => c.Addresses)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_addresses");
    }
}
