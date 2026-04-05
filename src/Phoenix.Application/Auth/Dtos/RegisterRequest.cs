using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Auth.Dtos;

/// <summary>
/// DTO de la requête d'inscription reçue par le contrôleur Auth.
/// Mappé vers <c>RegisterCommand</c> avant d'être dispatché via MediatR.
/// </summary>
/// <param name="Email">Adresse e-mail (identifiant unique).</param>
/// <param name="Password">Mot de passe en clair (min 8 caractères, 1 maj, 1 min, 1 chiffre).</param>
/// <param name="ConfirmPassword">Confirmation du mot de passe — doit être identique à <paramref name="Password"/>.</param>
/// <param name="FirstName">Prénom du client (max 100 caractères).</param>
/// <param name="LastName">Nom de famille (max 100 caractères).</param>
/// <param name="CompanyName">Raison sociale (optionnel, max 200 caractères).</param>
/// <param name="Segment">Segment professionnel du client.</param>
public sealed record RegisterRequest(
    string          Email,
    string          Password,
    string          ConfirmPassword,
    string          FirstName,
    string          LastName,
    string?         CompanyName,
    CustomerSegment Segment);
