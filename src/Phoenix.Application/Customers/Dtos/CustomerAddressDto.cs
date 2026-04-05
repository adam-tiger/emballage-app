namespace Phoenix.Application.Customers.Dtos;

/// <summary>
/// DTO représentant une adresse de livraison d'un client.
/// </summary>
/// <param name="Id">Identifiant unique de l'adresse.</param>
/// <param name="Label">Libellé fonctionnel (ex : "Mon restaurant").</param>
/// <param name="Street">Rue et numéro.</param>
/// <param name="City">Ville.</param>
/// <param name="PostalCode">Code postal.</param>
/// <param name="Country">Code pays ISO (ex : "FR").</param>
/// <param name="IsDefault">Indique si c'est l'adresse par défaut du client.</param>
public sealed record CustomerAddressDto(
    Guid   Id,
    string Label,
    string Street,
    string City,
    string PostalCode,
    string Country,
    bool   IsDefault);
