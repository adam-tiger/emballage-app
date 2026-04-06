namespace Phoenix.Api.Models.Auth;

/// <summary>
/// Modèle de requête pour la rotation du refresh token.
/// </summary>
/// <remarks>
/// Le refresh token est transmis via le Cookie HttpOnly <c>refreshToken</c> — pas dans le body.
/// Ce fichier est conservé pour documenter le contrat de l'endpoint <c>POST /api/v1/auth/refresh</c>.
/// </remarks>
public sealed record RefreshTokenRequestModel();
