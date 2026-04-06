using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Api.Models.Auth;

/// <summary>
/// Modèle de requête pour l'inscription d'un nouveau client.
/// </summary>
/// <param name="Email">Adresse e-mail (unique, max 256 caractères).</param>
/// <param name="Password">Mot de passe (min 8 car., 1 maj., 1 min., 1 chiffre, 1 symbole).</param>
/// <param name="ConfirmPassword">Confirmation du mot de passe — doit être identique à <paramref name="Password"/>.</param>
/// <param name="FirstName">Prénom (max 100 caractères).</param>
/// <param name="LastName">Nom de famille (max 100 caractères).</param>
/// <param name="CompanyName">Raison sociale (optionnel, max 200 caractères).</param>
/// <param name="Segment">Segment professionnel du client.</param>
public sealed record RegisterRequestModel(
    string          Email,
    string          Password,
    string          ConfirmPassword,
    string          FirstName,
    string          LastName,
    string?         CompanyName,
    CustomerSegment Segment);
