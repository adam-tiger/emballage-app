namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Port (interface) de l'unité de travail.
/// Encapsule le <c>DbContext.SaveChangesAsync</c> d'EF Core pour que la couche Application
/// reste indépendante de toute implémentation de persistance.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persiste toutes les modifications trackées dans la base de données.
    /// Doit être appelé en fin de handler Application après les opérations sur les agrégats.
    /// </summary>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>Nombre de lignes affectées en base.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
