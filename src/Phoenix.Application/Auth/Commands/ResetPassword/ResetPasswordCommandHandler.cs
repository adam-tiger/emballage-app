using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Common.Identity;

namespace Phoenix.Application.Auth.Commands.ResetPassword;

/// <summary>
/// Handler de la commande <see cref="ResetPasswordCommand"/>.
/// Valide le token Identity et réinitialise le mot de passe.
/// </summary>
public sealed class ResetPasswordCommandHandler(
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    /// <summary>Réinitialise le mot de passe et retourne <see cref="Unit.Value"/>.</summary>
    public async Task<Unit> Handle(ResetPasswordCommand command, CancellationToken ct)
    {
        // 1. Rechercher l'utilisateur
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
            throw new ValidationException(
            [
                new ValidationFailure("Token", "Lien invalide ou expiré.")
            ]);

        // 2. Réinitialiser le mot de passe via Identity
        var result = await userManager.ResetPasswordAsync(
            user, command.Token, command.NewPassword);

        if (!result.Succeeded)
            throw new ValidationException(
                result.Errors.Select(e =>
                    new ValidationFailure("Password", e.Description)));

        return Unit.Value;
    }
}
