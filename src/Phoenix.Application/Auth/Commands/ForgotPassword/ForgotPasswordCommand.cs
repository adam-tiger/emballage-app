using MediatR;

namespace Phoenix.Application.Auth.Commands.ForgotPassword;

/// <summary>
/// Commande de demande de réinitialisation de mot de passe.
/// Déclenche l'envoi d'un email contenant un lien de reset sécurisé.
/// </summary>
/// <remarks>
/// SÉCURITÉ : La réponse est identique que l'email existe ou non en base,
/// afin de ne pas divulguer l'existence d'un compte.
/// </remarks>
public sealed record ForgotPasswordCommand : IRequest<Unit>
{
    /// <summary>Adresse e-mail du compte pour lequel le reset est demandé.</summary>
    public required string Email { get; init; }
}
