using MediatR;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Auth.Commands.Register;

/// <summary>
/// Commande d'inscription d'un nouveau client sur Phoenix Emballages.
/// Crée simultanément un <c>ApplicationUser</c> (Identity) et un <c>Customer</c> (domaine).
/// </summary>
public sealed record RegisterCommand : IRequest<AuthResponse>
{
    /// <summary>Adresse e-mail du futur compte (max 256 caractères, unique).</summary>
    public required string Email { get; init; }

    /// <summary>Mot de passe en clair (min 8 car., 1 maj., 1 min., 1 chiffre).</summary>
    public required string Password { get; init; }

    /// <summary>Confirmation du mot de passe — doit être identique à <see cref="Password"/>.</summary>
    public required string ConfirmPassword { get; init; }

    /// <summary>Prénom (max 100 caractères).</summary>
    public required string FirstName { get; init; }

    /// <summary>Nom de famille (max 100 caractères).</summary>
    public required string LastName { get; init; }

    /// <summary>Raison sociale (optionnel, max 200 caractères).</summary>
    public string? CompanyName { get; init; }

    /// <summary>Segment professionnel du client.</summary>
    public required CustomerSegment Segment { get; init; }
}
