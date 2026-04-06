namespace Phoenix.Application.Auth.Dtos;

/// <summary>
/// Réponse interne retournée par les handlers MediatR après une authentification réussie.
/// Contient le refresh token pour que le contrôleur API puisse le placer dans un Cookie HttpOnly.
/// Le refresh token ne doit JAMAIS être sérialisé dans le corps de la réponse HTTP client.
/// </summary>
/// <param name="AccessToken">JWT access token signé (durée de vie : 15 minutes).</param>
/// <param name="RefreshToken">
/// Refresh token opaque (usage interne uniquement — placé dans Cookie HttpOnly par le contrôleur).
/// </param>
/// <param name="ExpiresIn">Durée de validité de l'access token en secondes (ex : 900).</param>
/// <param name="User">Profil complet de l'utilisateur authentifié.</param>
public sealed record AuthResponse(
    string         AccessToken,
    string         RefreshToken,
    int            ExpiresIn,
    UserProfileDto User);
