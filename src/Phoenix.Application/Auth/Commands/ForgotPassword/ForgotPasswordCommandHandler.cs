using MediatR;
using Microsoft.AspNetCore.Identity;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Application.Auth.Commands.ForgotPassword;

/// <summary>
/// Handler de la commande <see cref="ForgotPasswordCommand"/>.
/// Génère le lien de reset et envoie l'email si le compte existe et est confirmé.
/// </summary>
/// <remarks>
/// SÉCURITÉ : Retourne toujours <see cref="Unit.Value"/> — ne révèle jamais
/// si l'adresse e-mail est associée à un compte existant.
/// </remarks>
public sealed class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailService                emailService)
    : IRequestHandler<ForgotPasswordCommand, Unit>
{
    /// <summary>
    /// Génère et envoie le lien de réinitialisation si le compte est valide.
    /// Retourne toujours <see cref="Unit.Value"/> sans révéler l'existence du compte.
    /// </summary>
    public async Task<Unit> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        // Sécurité : ne pas révéler si l'email existe ou non
        if (user is not null && user.EmailConfirmed)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink =
                $"https://phoenix-emballages.fr/reset-password" +
                $"?token={Uri.EscapeDataString(token)}" +
                $"&email={Uri.EscapeDataString(command.Email)}";

            await emailService.SendPasswordResetEmailAsync(
                command.Email, resetLink, ct);
        }

        // Toujours retourner Unit.Value — même si le compte est introuvable
        return Unit.Value;
    }
}
