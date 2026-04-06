namespace Phoenix.Api.Models.Auth;

/// <summary>
/// Modèle de requête pour la connexion d'un utilisateur existant.
/// </summary>
/// <param name="Email">Adresse e-mail du compte.</param>
/// <param name="Password">Mot de passe en clair.</param>
/// <param name="RememberMe">
/// Si <c>true</c>, prolonge la durée de vie du cookie refresh token.
/// Valeur par défaut : <c>false</c>.
/// </param>
public sealed record LoginRequestModel(
    string Email,
    string Password,
    bool   RememberMe = false);
