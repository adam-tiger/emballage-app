namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Résultat de la génération d'un couple access token / refresh token.
/// </summary>
/// <param name="AccessToken">JWT access token signé (durée de vie courte, 15 minutes).</param>
/// <param name="RefreshToken">Refresh token opaque (durée de vie longue, 7 jours).</param>
/// <param name="AccessTokenExpiresAt">Date d'expiration UTC de l'access token.</param>
/// <param name="RefreshTokenExpiresAt">Date d'expiration UTC du refresh token.</param>
public record TokenResult(
    string   AccessToken,
    string   RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);

/// <summary>
/// Claims extraits d'un JWT valide ou fournis à la génération d'un token.
/// </summary>
/// <param name="UserId">Identifiant unique de l'utilisateur applicatif (sub claim).</param>
/// <param name="Email">Adresse e-mail de l'utilisateur (email claim).</param>
/// <param name="FirstName">Prénom de l'utilisateur (given_name claim).</param>
/// <param name="LastName">Nom de famille de l'utilisateur (family_name claim).</param>
/// <param name="Roles">Rôles applicatifs de l'utilisateur (role claim, ex : "Admin", "Customer").</param>
public record UserClaims(
    string   UserId,
    string   Email,
    string   FirstName,
    string   LastName,
    string[] Roles);

/// <summary>
/// Port (interface) du service de génération et de validation des tokens JWT.
/// Implémenté dans la couche Infrastructure avec <c>System.IdentityModel.Tokens.Jwt</c>.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Génère un couple access token JWT + refresh token opaque pour l'utilisateur donné.
    /// </summary>
    /// <param name="claims">Claims à embarquer dans le JWT (sub, email, rôles, etc.).</param>
    /// <returns>
    /// Un <see cref="TokenResult"/> contenant les deux tokens et leurs dates d'expiration.
    /// </returns>
    TokenResult GenerateTokens(UserClaims claims);

    /// <summary>
    /// Valide un access token JWT et extrait les claims qu'il contient.
    /// </summary>
    /// <param name="token">Le JWT sous forme de chaîne Bearer.</param>
    /// <returns>
    /// Les <see cref="UserClaims"/> extraits si le token est valide et non expiré ;
    /// <c>null</c> si le token est invalide, expiré ou falsifié.
    /// </returns>
    UserClaims? ValidateAccessToken(string token);

    /// <summary>
    /// Génère un refresh token opaque cryptographiquement sécurisé.
    /// Ce token est stocké hashé en base via <see cref="IRefreshTokenStore"/>.
    /// </summary>
    /// <returns>Chaîne aléatoire encodée en Base64 Url-safe.</returns>
    string GenerateRefreshToken();
}
