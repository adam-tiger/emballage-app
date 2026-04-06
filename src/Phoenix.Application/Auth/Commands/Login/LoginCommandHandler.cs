using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Application.Auth.Commands.Login;

/// <summary>
/// Handler de la commande <see cref="LoginCommand"/>.
/// Vérifie les credentials, applique le lockout, génère et stocke les tokens.
/// </summary>
public sealed class LoginCommandHandler(
    UserManager<ApplicationUser>   userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenService               jwtTokenService,
    IRefreshTokenStore             refreshTokenStore)
    : IRequestHandler<LoginCommand, AuthResponse>
{
    /// <summary>Authentifie l'utilisateur et retourne un <see cref="AuthResponse"/>.</summary>
    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken ct)
    {
        // 1. Rechercher l'utilisateur — message générique pour ne pas révéler l'existence
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
            throw new ValidationException(
            [
                new ValidationFailure("INVALID_CREDENTIALS", "Email ou mot de passe incorrect.")
            ]);

        // 2. Vérifier le mot de passe avec gestion du lockout
        var result = await signInManager.CheckPasswordSignInAsync(
            user, command.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            var message = result.IsLockedOut
                ? "Compte temporairement bloqué. Réessayez dans quelques minutes."
                : "Email ou mot de passe incorrect.";

            throw new ValidationException(
            [
                new ValidationFailure("INVALID_CREDENTIALS", message)
            ]);
        }

        // 3. Récupérer les rôles
        var roles = await userManager.GetRolesAsync(user);

        // 4. Construire les claims
        var claims = new UserClaims(
            UserId:    user.Id,
            Email:     user.Email!,
            FirstName: user.FirstName,
            LastName:  user.LastName,
            Roles:     roles.ToArray());

        // 5. Générer les tokens
        var tokens = jwtTokenService.GenerateTokens(claims);

        // 6. Stocker le refresh token
        await refreshTokenStore.StoreAsync(new RefreshTokenData(
            Token:     tokens.RefreshToken,
            UserId:    user.Id,
            ExpiresAt: tokens.RefreshTokenExpiresAt,
            IsRevoked: false,
            CreatedAt: DateTime.UtcNow), ct);

        // 7. Retourner AuthResponse
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
