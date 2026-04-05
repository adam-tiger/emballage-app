using Microsoft.AspNetCore.Identity;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Common.Identity;

/// <summary>
/// Utilisateur applicatif Phoenix — étend <see cref="IdentityUser"/> avec les propriétés
/// métier nécessaires à l'authentification et à la personnalisation de l'espace client.
/// </summary>
/// <remarks>
/// Cette classe est définie dans la couche Application pour permettre aux handlers MediatR
/// d'utiliser directement <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/>
/// sans dépendance sur la couche Infrastructure.
/// La couche Infrastructure hérite de cette classe (ou l'utilise telle quelle)
/// pour la configuration EF Core et Identity.
/// </remarks>
public class ApplicationUser : IdentityUser
{
    /// <summary>Prénom de l'utilisateur. Maximum 100 caractères.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Nom de famille de l'utilisateur. Maximum 100 caractères.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Raison sociale ou nom commercial (optionnel). Maximum 200 caractères.</summary>
    public string? CompanyName { get; set; }

    /// <summary>Segment professionnel de l'utilisateur (ex : FastFood, BakeryPastry).</summary>
    public CustomerSegment Segment { get; set; } = CustomerSegment.Other;

    /// <summary>Indique si le compte est actif. Mis à <c>false</c> lors d'une désactivation.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Date de création du compte (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Identifiant du profil client métier associé (<see cref="Phoenix.Domain.Customers.Entities.Customer"/>).
    /// <c>null</c> si le compte n'a pas encore de profil client (ex : compte Admin/Employee).
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Nom complet calculé (prénom + nom).</summary>
    public string FullName => $"{FirstName} {LastName}";
}
