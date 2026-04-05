using MediatR;
using Phoenix.Application.Auth.Dtos;

namespace Phoenix.Application.Auth.Commands.Login;

/// <summary>
/// Commande de connexion d'un utilisateur existant.
/// Vérifie les credentials et génère un couple access token / refresh token.
/// </summary>
public sealed record LoginCommand : IRequest<AuthResponse>
{
    /// <summary>Adresse e-mail du compte.</summary>
    public required string Email { get; init; }

    /// <summary>Mot de passe en clair.</summary>
    public required string Password { get; init; }

    /// <summary>
    /// Si <c>true</c>, indique que l'utilisateur accepte un cookie de refresh token
    /// avec une durée de vie prolongée. Valeur par défaut : <c>false</c>.
    /// </summary>
    public bool RememberMe { get; init; } = false;
}
