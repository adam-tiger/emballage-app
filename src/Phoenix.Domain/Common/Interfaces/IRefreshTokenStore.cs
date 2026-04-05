namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Données associées à un refresh token stocké en base de données.
/// </summary>
/// <param name="Token">
/// Valeur du refresh token (hashée en base via SHA-256, transmise en clair au client
/// via Cookie HttpOnly).
/// </param>
/// <param name="UserId">Identifiant de l'utilisateur applicatif propriétaire du token.</param>
/// <param name="ExpiresAt">Date d'expiration UTC du refresh token (7 jours après émission).</param>
/// <param name="IsRevoked">
/// <c>true</c> si le token a été explicitement révoqué (logout ou rotation).
/// </param>
/// <param name="CreatedAt">Date de création UTC du token.</param>
public record RefreshTokenData(
    string   Token,
    string   UserId,
    DateTime ExpiresAt,
    bool     IsRevoked,
    DateTime CreatedAt);

/// <summary>
/// Port (interface) du magasin de refresh tokens.
/// Implémenté dans la couche Infrastructure (EF Core + PostgreSQL).
/// </summary>
/// <remarks>
/// Stratégie de rotation : à chaque appel à <c>/auth/refresh</c>, l'ancien token est révoqué
/// et un nouveau est émis. Un token révoqué ou expiré est rejeté et force un nouveau login.
/// </remarks>
public interface IRefreshTokenStore
{
    /// <summary>
    /// Persiste un nouveau refresh token en base de données.
    /// </summary>
    /// <param name="data">Données du token à stocker.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task StoreAsync(RefreshTokenData data, CancellationToken ct = default);

    /// <summary>
    /// Récupère les données d'un refresh token par sa valeur en clair.
    /// </summary>
    /// <param name="token">Valeur du token telle que transmise par le client.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>
    /// Les <see cref="RefreshTokenData"/> associées si le token existe ; <c>null</c> sinon.
    /// </returns>
    Task<RefreshTokenData?> GetAsync(string token, CancellationToken ct = default);

    /// <summary>
    /// Révoque un refresh token spécifique (ex : logout simple ou rotation).
    /// </summary>
    /// <param name="token">Valeur du token à révoquer.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task RevokeAsync(string token, CancellationToken ct = default);

    /// <summary>
    /// Révoque tous les refresh tokens actifs d'un utilisateur donné.
    /// Utilisé lors d'un logout global ou d'une réinitialisation de mot de passe.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur applicatif.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task RevokeAllForUserAsync(string userId, CancellationToken ct = default);
}
