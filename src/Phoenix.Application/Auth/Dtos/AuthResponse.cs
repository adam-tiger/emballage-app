namespace Phoenix.Application.Auth.Dtos;

/// <summary>
/// Réponse retournée après une authentification réussie (inscription, connexion, refresh).
/// Le refresh token est transmis séparément via un Cookie HttpOnly — absent de ce DTO.
/// </summary>
/// <param name="AccessToken">JWT access token signé (durée de vie : 15 minutes).</param>
/// <param name="ExpiresIn">Durée de validité de l'access token en secondes (ex : 900).</param>
/// <param name="User">Profil complet de l'utilisateur authentifié.</param>
public sealed record AuthResponse(
    string         AccessToken,
    int            ExpiresIn,
    UserProfileDto User);
