using MediatR;

namespace Phoenix.Application.Auth.Commands.Logout;

/// <summary>
/// Commande de déconnexion.
/// Révoque le refresh token courant et tous les tokens actifs de l'utilisateur.
/// </summary>
public sealed record LogoutCommand : IRequest<Unit>
{
    /// <summary>
    /// Refresh token à révoquer (extrait du cookie HttpOnly par le contrôleur).
    /// Optionnel — si absent, seule la révocation globale est effectuée.
    /// </summary>
    public string? RefreshToken { get; init; }
}
