using MediatR;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Application.Auth.Commands.Logout;

/// <summary>
/// Handler de la commande <see cref="LogoutCommand"/>.
/// Révoque le refresh token spécifique et tous les tokens actifs de l'utilisateur courant.
/// </summary>
public sealed class LogoutCommandHandler(
    IRefreshTokenStore refreshTokenStore,
    ICurrentUserService currentUserService)
    : IRequestHandler<LogoutCommand, Unit>
{
    /// <summary>Révoque les tokens et retourne <see cref="Unit.Value"/>.</summary>
    public async Task<Unit> Handle(LogoutCommand command, CancellationToken ct)
    {
        // 1. Révoquer le refresh token spécifique si fourni
        if (!string.IsNullOrWhiteSpace(command.RefreshToken))
            await refreshTokenStore.RevokeAsync(command.RefreshToken, ct);

        // 2. Révoquer tous les tokens de l'utilisateur courant (logout global)
        if (currentUserService.UserId is not null)
            await refreshTokenStore.RevokeAllForUserAsync(
                currentUserService.UserId.ToString()!, ct);

        return Unit.Value;
    }
}
