namespace Phoenix.Application.Auth.Dtos;

/// <summary>
/// Profil complet de l'utilisateur authentifié courant.
/// Retourné par <c>GET /api/v1/auth/me</c> et inclus dans chaque <see cref="AuthResponse"/>.
/// </summary>
/// <param name="Id">Identifiant unique de l'<c>ApplicationUser</c> (string GUID Identity).</param>
/// <param name="Email">Adresse e-mail de l'utilisateur.</param>
/// <param name="FirstName">Prénom.</param>
/// <param name="LastName">Nom de famille.</param>
/// <param name="FullName">Nom complet (prénom + nom).</param>
/// <param name="CompanyName">Raison sociale (optionnel).</param>
/// <param name="Segment">Valeur string du segment professionnel (ex : "FastFood").</param>
/// <param name="Roles">Rôles applicatifs (ex : ["Customer"], ["Admin", "Employee"]).</param>
/// <param name="IsActive">Indique si le compte est actif.</param>
/// <param name="CreatedAtUtc">Date de création du compte (UTC).</param>
public sealed record UserProfileDto(
    string   Id,
    string   Email,
    string   FirstName,
    string   LastName,
    string   FullName,
    string?  CompanyName,
    string   Segment,
    string[] Roles,
    bool     IsActive,
    DateTime CreatedAtUtc);
