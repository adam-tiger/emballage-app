using MediatR;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Customers.Commands.UpdateProfile;

/// <summary>
/// Commande de mise à jour du profil client authentifié.
/// L'identité est résolue depuis <c>ICurrentUserService</c> dans le handler.
/// </summary>
public sealed record UpdateCustomerProfileCommand : IRequest<Unit>
{
    /// <summary>Nouveau prénom (max 100 caractères).</summary>
    public required string FirstName { get; init; }

    /// <summary>Nouveau nom de famille (max 100 caractères).</summary>
    public required string LastName { get; init; }

    /// <summary>Nouvelle raison sociale (optionnel, max 200 caractères).</summary>
    public string? CompanyName { get; init; }

    /// <summary>Nouveau segment professionnel.</summary>
    public required CustomerSegment Segment { get; init; }
}
