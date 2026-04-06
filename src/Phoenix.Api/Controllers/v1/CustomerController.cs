using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Phoenix.Api.Models;
using Phoenix.Application.Customers.Commands.AddAddress;
using Phoenix.Application.Customers.Commands.SetDefaultAddress;
using Phoenix.Application.Customers.Commands.UpdateProfile;
using Phoenix.Application.Customers.Dtos;
using Phoenix.Application.Customers.Queries.GetDashboard;
using Phoenix.Application.Customers.Queries.GetProfile;

namespace Phoenix.Api.Controllers.v1;

/// <summary>
/// Contrôleur gérant le profil client, les adresses et le tableau de bord Phoenix.
/// Tous les endpoints requièrent un JWT valide avec le rôle <c>Customer</c>, <c>Admin</c> ou <c>Employee</c>.
/// </summary>
[ApiController]
[Route("api/v1/customer")]
[Produces("application/json")]
[Authorize(Roles = "Customer,Admin,Employee")]
public sealed class CustomerController(IMediator mediator) : ControllerBase
{
    // ── GET /api/v1/customer/profile ─────────────────────────────────────────

    /// <summary>Retourne le profil complet du client authentifié courant.</summary>
    /// <remarks>
    /// Inclut les informations personnelles, le segment professionnel et la liste des adresses.
    /// </remarks>
    /// <response code="200">Profil client complet avec adresses.</response>
    /// <response code="401">Token JWT absent ou expiré.</response>
    /// <response code="403">Rôle insuffisant.</response>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(CustomerProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerProfileQuery(), ct);
        return Ok(result);
    }

    // ── PUT /api/v1/customer/profile ─────────────────────────────────────────

    /// <summary>Met à jour le profil du client authentifié courant.</summary>
    /// <param name="command">Nouvelles valeurs du profil (prénom, nom, raison sociale, segment).</param>
    /// <response code="204">Profil mis à jour avec succès.</response>
    /// <response code="400">Données invalides.</response>
    /// <response code="404">Profil client introuvable.</response>
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateCustomerProfileCommand command,
        CancellationToken ct)
    {
        await mediator.Send(command, ct);
        return NoContent();
    }

    // ── GET /api/v1/customer/dashboard ───────────────────────────────────────

    /// <summary>Retourne le tableau de bord du client authentifié courant.</summary>
    /// <remarks>
    /// Contient un résumé des commandes, devis et de l'adresse de livraison par défaut.
    /// Les compteurs sont à 0 tant que les modules Commandes (Module 4) et Devis (Module 5)
    /// ne sont pas développés.
    /// </remarks>
    /// <response code="200">Tableau de bord du client.</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(CustomerDashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerDashboardQuery(), ct);
        return Ok(result);
    }

    // ── GET /api/v1/customer/addresses ───────────────────────────────────────

    /// <summary>Retourne la liste des adresses de livraison du client courant.</summary>
    /// <response code="200">Liste des adresses (vide si aucune adresse enregistrée).</response>
    [HttpGet("addresses")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerAddressDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAddresses(CancellationToken ct)
    {
        var profile = await mediator.Send(new GetCustomerProfileQuery(), ct);
        return Ok(profile.Addresses);
    }

    // ── POST /api/v1/customer/addresses ──────────────────────────────────────

    /// <summary>Ajoute une nouvelle adresse de livraison au profil client.</summary>
    /// <remarks>
    /// Un client peut avoir au maximum 5 adresses.
    /// La première adresse ajoutée devient automatiquement l'adresse par défaut.
    /// </remarks>
    /// <param name="command">Données de la nouvelle adresse.</param>
    /// <response code="201">Adresse créée — retourne l'identifiant de la nouvelle adresse.</response>
    /// <response code="400">Données invalides ou limite de 5 adresses atteinte.</response>
    [HttpPost("addresses")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAddress(
        [FromBody] AddCustomerAddressCommand command,
        CancellationToken ct)
    {
        var addressId = await mediator.Send(command, ct);
        return Created($"api/v1/customer/addresses/{addressId}", addressId);
    }

    // ── PUT /api/v1/customer/addresses/{addressId:guid}/default ─────────────

    /// <summary>Définit une adresse existante comme adresse de livraison par défaut.</summary>
    /// <param name="addressId">Identifiant de l'adresse à promouvoir par défaut.</param>
    /// <response code="204">Adresse par défaut mise à jour avec succès.</response>
    /// <response code="404">Adresse introuvable ou n'appartient pas au client courant.</response>
    [HttpPut("addresses/{addressId:guid}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultAddress(
        [FromRoute] Guid addressId,
        CancellationToken ct)
    {
        await mediator.Send(new SetDefaultAddressCommand { AddressId = addressId }, ct);
        return NoContent();
    }
}
