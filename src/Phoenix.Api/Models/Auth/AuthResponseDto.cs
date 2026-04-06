using Phoenix.Application.Auth.Dtos;

namespace Phoenix.Api.Models.Auth;

/// <summary>
/// DTO de réponse d'authentification envoyé au client HTTP.
/// </summary>
/// <remarks>
/// Différent de <see cref="Phoenix.Application.Auth.Dtos.AuthResponse"/> (usage interne) :
/// ce DTO ne contient PAS le refresh token — celui-ci est transmis uniquement
/// via un Cookie HttpOnly <c>refreshToken</c> pour des raisons de sécurité (protection XSS).
/// </remarks>
/// <param name="AccessToken">JWT access token signé (durée de vie : 15 minutes). À conserver en mémoire côté client.</param>
/// <param name="ExpiresIn">Durée de validité de l'access token en secondes (ex : 900).</param>
/// <param name="User">Profil complet de l'utilisateur authentifié.</param>
public sealed record AuthResponseDto(
    string         AccessToken,
    int            ExpiresIn,
    UserProfileDto User);
