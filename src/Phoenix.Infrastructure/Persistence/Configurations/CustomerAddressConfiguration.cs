using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phoenix.Domain.Customers.Entities;

namespace Phoenix.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité <see cref="CustomerAddress"/>.
/// </summary>
public sealed class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("customer_addresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.CustomerId)
            .IsRequired();

        builder.Property(a => a.Label)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Street)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.PostalCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(a => a.Country)
            .HasMaxLength(2)
            .HasDefaultValue("FR")
            .IsRequired();

        builder.Property(a => a.IsDefault)
            .HasDefaultValue(false);

        builder.Property(a => a.CreatedAtUtc)
            .IsRequired();
    }
}
