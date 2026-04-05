namespace Phoenix.Application.Auth.Dtos;

/// <summary>
/// DTO de la requête de connexion reçue par le contrôleur Auth.
/// Mappé vers <c>LoginCommand</c> avant d'être dispatché via MediatR.
/// </summary>
/// <param name="Email">Adresse e-mail de l'utilisateur.</param>
/// <param name="Password">Mot de passe en clair (jamais persisté).</param>
/// <param name="RememberMe">
/// Si <c>true</c>, la durée du cookie de refresh token peut être prolongée.
/// </param>
public sealed record LoginRequest(
    string Email,
    string Password,
    bool   RememberMe = false);
