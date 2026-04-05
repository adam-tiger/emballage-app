using MediatR;

namespace Phoenix.Application.Customers.Commands.AddAddress;

/// <summary>
/// Commande d'ajout d'une adresse de livraison au profil client courant.
/// Retourne l'identifiant de la nouvelle adresse créée.
/// </summary>
public sealed record AddCustomerAddressCommand : IRequest<Guid>
{
    /// <summary>Libellé fonctionnel de l'adresse (ex : "Mon restaurant", "Entrepôt").</summary>
    public required string Label { get; init; }

    /// <summary>Numéro et nom de la rue.</summary>
    public required string Street { get; init; }

    /// <summary>Ville.</summary>
    public required string City { get; init; }

    /// <summary>Code postal (5 chiffres pour la France).</summary>
    public required string PostalCode { get; init; }

    /// <summary>Code pays ISO 3166-1 alpha-2. Par défaut : <c>"FR"</c>.</summary>
    public string Country { get; init; } = "FR";

    /// <summary>
    /// Si <c>true</c>, définit cette adresse comme adresse par défaut.
    /// Ignoré si c'est la première adresse du client (automatiquement défaut).
    /// </summary>
    public bool IsDefault { get; init; } = false;
}
