using MediatR;

namespace Phoenix.Application.Auth.Commands.ResetPassword;

/// <summary>
/// Commande de réinitialisation du mot de passe via le lien de reset.
/// </summary>
public sealed record ResetPasswordCommand : IRequest<Unit>
{
    /// <summary>Adresse e-mail du compte concerné.</summary>
    public required string Email { get; init; }

    /// <summary>Token de reset généré par ASP.NET Identity (extrait du lien envoyé par email).</summary>
    public required string Token { get; init; }

    /// <summary>Nouveau mot de passe (min 8 car., 1 maj., 1 min., 1 chiffre).</summary>
    public required string NewPassword { get; init; }

    /// <summary>Confirmation du nouveau mot de passe.</summary>
    public required string ConfirmNewPassword { get; init; }
}
