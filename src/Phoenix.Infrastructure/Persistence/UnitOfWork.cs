using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Infrastructure.Persistence;

/// <summary>
/// Implémentation de <see cref="IUnitOfWork"/> qui délègue à <see cref="PhoenixDbContext"/>.
/// Encapsule <c>DbContext.SaveChangesAsync</c> pour maintenir l'indépendance de la couche Application.
/// </summary>
internal sealed class UnitOfWork(PhoenixDbContext context) : IUnitOfWork
{
    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
