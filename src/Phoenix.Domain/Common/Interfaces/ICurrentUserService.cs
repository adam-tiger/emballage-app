namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Port (interface) fournissant le contexte de l'utilisateur authentifié courant.
/// Résolu depuis le JWT dans Phoenix.Infrastructure via <c>IHttpContextAccessor</c>.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Identifiant unique de l'utilisateur authentifié.
    /// <c>null</c> si la requête est anonyme.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Adresse e-mail de l'utilisateur authentifié.
    /// <c>null</c> si la requête est anonyme.
    /// </summary>
    string? UserEmail { get; }

    /// <summary>
    /// Indique si l'utilisateur courant est authentifié.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Vérifie si l'utilisateur courant possède le rôle spécifié.
    /// </summary>
    /// <param name="role">Nom du rôle à vérifier (ex : "Admin", "SalesManager").</param>
    /// <returns><c>true</c> si l'utilisateur possède le rôle.</returns>
    bool IsInRole(string role);
}
