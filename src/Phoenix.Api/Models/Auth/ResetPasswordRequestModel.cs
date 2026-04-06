namespace Phoenix.Api.Models.Auth;

/// <summary>
/// Modèle de requête pour la réinitialisation du mot de passe via le lien de reset.
/// </summary>
/// <param name="Email">Adresse e-mail du compte concerné.</param>
/// <param name="Token">Token de réinitialisation généré par ASP.NET Identity (extrait du lien).</param>
/// <param name="NewPassword">Nouveau mot de passe (min 8 car., 1 maj., 1 min., 1 chiffre, 1 symbole).</param>
/// <param name="ConfirmNewPassword">Confirmation du nouveau mot de passe.</param>
public sealed record ResetPasswordRequestModel(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmNewPassword);
