using MediatR;
using Phoenix.Application.Auth.Dtos;

namespace Phoenix.Application.Auth.Commands.RefreshToken;

/// <summary>
/// Commande de rotation du refresh token.
/// Révoque l'ancien token et en émet un nouveau couple access/refresh.
/// </summary>
public sealed record RefreshTokenCommand : IRequest<AuthResponse>
{
    /// <summary>
    /// Valeur du refresh token telle que transmise par le client
    /// (extraite du cookie HttpOnly par le contrôleur).
    /// </summary>
    public required string RefreshToken { get; init; }
}
