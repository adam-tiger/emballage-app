using MediatR;

namespace Phoenix.Application.Customers.Commands.SetDefaultAddress;

/// <summary>
/// Commande de définition de l'adresse de livraison par défaut du client courant.
/// </summary>
public sealed record SetDefaultAddressCommand : IRequest<Unit>
{
    /// <summary>Identifiant de l'adresse à promouvoir comme adresse par défaut.</summary>
    public required Guid AddressId { get; init; }
}
