namespace Phoenix.Api.Models.Auth;

/// <summary>
/// Modèle de requête pour la demande de réinitialisation du mot de passe.
/// </summary>
/// <param name="Email">Adresse e-mail du compte concerné.</param>
public sealed record ForgotPasswordRequestModel(string Email);
