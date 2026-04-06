using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Application.Auth.Commands.RefreshToken;

/// <summary>
/// Handler de la commande <see cref="RefreshTokenCommand"/>.
/// Applique la stratégie de rotation : révoque l'ancien token, émet un nouveau couple.
/// </summary>
public sealed class RefreshTokenCommandHandler(
    IRefreshTokenStore            refreshTokenStore,
    UserManager<ApplicationUser>  userManager,
    IJwtTokenService              jwtTokenService)
    : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    /// <summary>Effectue la rotation du refresh token et retourne un nouvel <see cref="AuthResponse"/>.</summary>
    public async Task<AuthResponse> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        // 1. Récupérer les données du token
        var tokenData = await refreshTokenStore.GetAsync(command.RefreshToken, ct);
        if (tokenData is null)
            throw new ValidationException(
            [
                new ValidationFailure("RefreshToken", "Token invalide.")
            ]);

        // 2. Vérifier la révocation
        if (tokenData.IsRevoked)
            throw new ValidationException(
            [
                new ValidationFailure("RefreshToken", "Token révoqué.")
            ]);

        // 3. Vérifier l'expiration
        if (tokenData.ExpiresAt < DateTime.UtcNow)
            throw new ValidationException(
            [
                new ValidationFailure("RefreshToken", "Token expiré.")
            ]);

        // 4. Révoquer l'ancien token (rotation)
        await refreshTokenStore.RevokeAsync(command.RefreshToken, ct);

        // 5. Récupérer l'utilisateur
        var user = await userManager.FindByIdAsync(tokenData.UserId);
        if (user is null)
            throw new ValidationException(
            [
                new ValidationFailure("RefreshToken", "Utilisateur introuvable.")
            ]);

        // 6. Générer les nouveaux tokens
        var roles  = await userManager.GetRolesAsync(user);
        var claims = new UserClaims(
            UserId:    user.Id,
            Email:     user.Email!,
            FirstName: user.FirstName,
            LastName:  user.LastName,
            Roles:     roles.ToArray());

        var tokens = jwtTokenService.GenerateTokens(claims);

        // 7. Stocker le nouveau refresh token
        await refreshTokenStore.StoreAsync(new RefreshTokenData(
            Token:     tokens.RefreshToken,
            UserId:    user.Id,
            ExpiresAt: tokens.RefreshTokenExpiresAt,
            IsRevoked: false,
            CreatedAt: DateTime.UtcNow), ct);

        // 8. Retourner AuthResponse
        var profile = new UserProfileDto(
            Id:          user.Id,
            Email:       user.Email!,
            FirstName:   user.FirstName,
            LastName:    user.LastName,
            FullName:    user.FullName,
            CompanyName: user.CompanyName,
            Segment:     user.Segment.ToString(),
            Roles:       roles.ToArray(),
            IsActive:    user.IsActive,
            CreatedAtUtc: user.CreatedAtUtc);

        var expiresIn = (int)(tokens.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds;
        return new AuthResponse(tokens.AccessToken, tokens.RefreshToken, expiresIn, profile);
    }
}
