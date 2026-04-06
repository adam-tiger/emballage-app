using Microsoft.EntityFrameworkCore;
using Phoenix.Domain.Customers.Entities;
using Phoenix.Domain.Customers.Repositories;
using Phoenix.Infrastructure.Persistence;

namespace Phoenix.Infrastructure.Repositories;

/// <summary>
/// Implémentation EF Core de <see cref="ICustomerRepository"/>.
/// </summary>
public sealed class CustomerRepository(PhoenixDbContext db) : ICustomerRepository
{
    /// <inheritdoc />
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <inheritdoc />
    public async Task<Customer?> GetByApplicationUserIdAsync(Guid userId, CancellationToken ct = default)
        => await db.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId, ct);

    /// <inheritdoc />
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await db.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant(), ct);

    /// <inheritdoc />
    public async Task AddAsync(Customer customer, CancellationToken ct = default)
        => await db.Customers.AddAsync(customer, ct);

    /// <inheritdoc />
    public Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        db.Customers.Update(customer);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string email, CancellationToken ct = default)
        => await db.Customers
            .AnyAsync(c => c.Email == email.ToLowerInvariant(), ct);
}
