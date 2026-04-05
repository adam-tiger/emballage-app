using Phoenix.Domain.Customers.Entities;

namespace Phoenix.Domain.Customers.Repositories;

/// <summary>
/// Port (interface) du repository pour l'agrégat <see cref="Customer"/>.
/// Implémenté dans la couche Infrastructure (EF Core + PostgreSQL).
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Récupère un client par son identifiant de domaine.
    /// </summary>
    /// <param name="id">Identifiant unique du <see cref="Customer"/>.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>Le <see cref="Customer"/> trouvé, ou <c>null</c> s'il n'existe pas.</returns>
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Récupère un client par l'identifiant de son utilisateur applicatif ASP.NET Identity.
    /// </summary>
    /// <param name="userId">Identifiant de l'<c>ApplicationUser</c> associé.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>Le <see cref="Customer"/> trouvé, ou <c>null</c> s'il n'existe pas.</returns>
    Task<Customer?> GetByApplicationUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Récupère un client par son adresse e-mail (insensible à la casse).
    /// </summary>
    /// <param name="email">Adresse e-mail du client.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>Le <see cref="Customer"/> trouvé, ou <c>null</c> s'il n'existe pas.</returns>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Persiste un nouveau client en base de données.
    /// </summary>
    /// <param name="customer">Instance de <see cref="Customer"/> à ajouter.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task AddAsync(Customer customer, CancellationToken ct = default);

    /// <summary>
    /// Marque un client existant comme modifié pour que EF Core le persiste
    /// lors du prochain <c>SaveChangesAsync</c>.
    /// </summary>
    /// <param name="customer">Instance de <see cref="Customer"/> à mettre à jour.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task UpdateAsync(Customer customer, CancellationToken ct = default);

    /// <summary>
    /// Vérifie si un client avec l'adresse e-mail donnée existe déjà en base.
    /// Utilisé avant l'inscription pour garantir l'unicité de l'e-mail.
    /// </summary>
    /// <param name="email">Adresse e-mail à vérifier.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns><c>true</c> si un client avec cet e-mail existe déjà.</returns>
    Task<bool> ExistsAsync(string email, CancellationToken ct = default);
}
