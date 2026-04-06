namespace Phoenix.Application.Auth.Dtos;

/// <summary>
/// DTO léger représentant uniquement les informations du token d'accès.
/// Utilisé dans les réponses ne nécessitant pas le profil utilisateur complet.
/// </summary>
/// <param name="AccessToken">JWT access token signé.</param>
/// <param name="TokenType">Type de token — toujours <c>"Bearer"</c>.</param>
/// <param name="ExpiresIn">Durée de validité en secondes (ex : 900).</param>
public sealed record TokenDto(
    string AccessToken,
    string TokenType,
    int    ExpiresIn);
