namespace Phoenix.Application.Customers.Dtos;

/// <summary>
/// DTO complet du profil client, incluant les adresses de livraison.
/// Retourné par <c>GET /api/v1/customers/me/profile</c>.
/// </summary>
/// <param name="Id">Identifiant unique du Customer (domaine).</param>
/// <param name="Email">Adresse e-mail.</param>
/// <param name="FirstName">Prénom.</param>
/// <param name="LastName">Nom de famille.</param>
/// <param name="FullName">Nom complet (prénom + nom).</param>
/// <param name="CompanyName">Raison sociale (optionnel).</param>
/// <param name="Segment">Valeur string du segment professionnel (ex : "FastFood").</param>
/// <param name="SegmentLabel">Libellé français du segment (ex : "Fast Food &amp; Burger").</param>
/// <param name="Addresses">Liste des adresses de livraison enregistrées.</param>
/// <param name="CreatedAtUtc">Date de création du profil (UTC).</param>
public sealed record CustomerProfileDto(
    Guid                            Id,
    string                          Email,
    string                          FirstName,
    string                          LastName,
    string                          FullName,
    string?                         CompanyName,
    string                          Segment,
    string                          SegmentLabel,
    IReadOnlyList<CustomerAddressDto> Addresses,
    DateTime                        CreatedAtUtc);
