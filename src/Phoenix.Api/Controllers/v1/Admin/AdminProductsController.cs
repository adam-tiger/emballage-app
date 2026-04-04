using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phoenix.Api.Models;
using Phoenix.Application.Products.Commands.AddPriceTier;
using Phoenix.Application.Products.Commands.AddProductVariant;
using Phoenix.Application.Products.Commands.CreateProduct;
using Phoenix.Application.Products.Commands.DeactivateProduct;
using Phoenix.Application.Products.Commands.UpdateProduct;
using Phoenix.Application.Products.Commands.UploadProductImage;
using Phoenix.Application.Products.Dtos;
using Phoenix.Application.Products.Queries.GetProductById;
using Phoenix.Application.Products.Queries.GetProductList;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Api.Controllers.v1.Admin;

/// <summary>
/// Endpoints d'administration du catalogue produit.
/// Accès restreint aux rôles <c>Admin</c> et <c>Employee</c>.
/// </summary>
[ApiController]
[Route("api/v1/admin/products")]
[Produces("application/json")]
[Authorize(Roles = "Admin,Employee")]
public sealed class AdminProductsController(IMediator mediator) : ControllerBase
{
    // ── Corps de requête pour les opérations avec paramètres de route ────────

    /// <summary>Corps de requête pour la mise à jour d'un produit.</summary>
    private sealed record UpdateProductBody(string NameFr, string DescriptionFr);

    /// <summary>Corps de requête pour l'ajout d'une variante d'impression.</summary>
    private sealed record AddProductVariantBody(
        string Sku,
        string NameFr,
        int MinimumOrderQuantity,
        PrintSide PrintSide,
        ColorCount ColorCount);

    /// <summary>Corps de requête pour l'ajout d'un palier tarifaire.</summary>
    private sealed record AddPriceTierBody(
        int MinQuantity,
        int? MaxQuantity,
        decimal UnitPriceHT);

    // ── Endpoints de consultation ────────────────────────────────────────────

    /// <summary>
    /// Retourne une liste paginée et filtrée de produits (actifs et/ou inactifs).
    /// </summary>
    /// <param name="page">Numéro de page (base 1). Défaut : 1.</param>
    /// <param name="pageSize">Nombre d'éléments par page. Défaut : 20.</param>
    /// <param name="sortBy">Propriété de tri. Défaut : "NameFr".</param>
    /// <param name="sortDir">Direction du tri : "asc" ou "desc". Défaut : "asc".</param>
    /// <param name="family">Filtre optionnel par famille de produits.</param>
    /// <param name="segment">Filtre optionnel par segment client.</param>
    /// <param name="isCustomizable">Filtre sur les produits personnalisables.</param>
    /// <param name="searchText">Recherche plein-texte.</param>
    /// <param name="isActive"><c>true</c> = actifs, <c>false</c> = inactifs, <c>null</c> = tous.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="200">Liste paginée des produits.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page              = 1,
        [FromQuery] int pageSize          = 20,
        [FromQuery] string sortBy         = "NameFr",
        [FromQuery] string sortDir        = "asc",
        [FromQuery] ProductFamily? family = null,
        [FromQuery] CustomerSegment? segment      = null,
        [FromQuery] bool? isCustomizable  = null,
        [FromQuery] string? searchText    = null,
        [FromQuery] bool? isActive        = null,
        CancellationToken ct              = default)
    {
        var result = await mediator.Send(new GetProductListQuery
        {
            Page           = page,
            PageSize       = pageSize,
            SortBy         = sortBy,
            SortDir        = sortDir,
            Family         = family,
            Segment        = segment,
            IsCustomizable = isCustomizable,
            SearchText     = searchText,
            IsActive       = isActive   // null = tous les produits (actifs + inactifs)
        }, ct);

        return Ok(result);
    }

    /// <summary>
    /// Retourne le détail complet d'un produit par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant unique du produit.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="200">Détail complet du produit.</response>
    /// <response code="404">Produit introuvable.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetProductByIdQuery { Id = id }, ct);
        return Ok(result);
    }

    // ── Endpoints de modification ────────────────────────────────────────────

    /// <summary>
    /// Crée un nouveau produit dans le catalogue.
    /// </summary>
    /// <param name="command">Données du produit à créer.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="201">Produit créé — retourne l'identifiant du nouveau produit.</response>
    /// <response code="400">Données invalides ou SKU déjà utilisé.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken ct = default)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetProductById), new { id }, id);
    }

    /// <summary>
    /// Met à jour le libellé et la description d'un produit existant.
    /// </summary>
    /// <param name="id">Identifiant du produit à modifier.</param>
    /// <param name="body">Nouvelles valeurs (NameFr, DescriptionFr).</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="204">Produit mis à jour avec succès.</response>
    /// <response code="400">Données invalides.</response>
    /// <response code="404">Produit introuvable.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductBody body,
        CancellationToken ct = default)
    {
        await mediator.Send(new UpdateProductCommand
        {
            Id            = id,
            NameFr        = body.NameFr,
            DescriptionFr = body.DescriptionFr
        }, ct);

        return NoContent();
    }

    /// <summary>
    /// Désactive un produit (soft-delete — le retire du catalogue public).
    /// </summary>
    /// <param name="id">Identifiant du produit à désactiver.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="204">Produit désactivé avec succès.</response>
    /// <response code="404">Produit introuvable.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateProduct(Guid id, CancellationToken ct = default)
    {
        await mediator.Send(new DeactivateProductCommand { Id = id }, ct);
        return NoContent();
    }

    // ── Endpoints de variantes ───────────────────────────────────────────────

    /// <summary>
    /// Ajoute une variante d'impression à un produit existant.
    /// </summary>
    /// <param name="id">Identifiant du produit parent.</param>
    /// <param name="body">Données de la variante (Sku, NameFr, MOQ, PrintSide, ColorCount).</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="201">Variante créée — retourne l'identifiant de la variante.</response>
    /// <response code="400">Données invalides ou SKU déjà utilisé.</response>
    [HttpPost("{id:guid}/variants")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddVariant(
        Guid id,
        [FromBody] AddProductVariantBody body,
        CancellationToken ct = default)
    {
        var variantId = await mediator.Send(new AddProductVariantCommand
        {
            ProductId            = id,
            Sku                  = body.Sku,
            NameFr               = body.NameFr,
            MinimumOrderQuantity = body.MinimumOrderQuantity,
            PrintSide            = body.PrintSide,
            ColorCount           = body.ColorCount
        }, ct);

        return Created($"api/v1/admin/products/{id}/variants/{variantId}", variantId);
    }

    // ── Endpoints de paliers tarifaires ─────────────────────────────────────

    /// <summary>
    /// Ajoute un palier tarifaire à une variante d'impression.
    /// </summary>
    /// <param name="id">Identifiant du produit parent.</param>
    /// <param name="variantId">Identifiant de la variante cible.</param>
    /// <param name="body">Données du palier (MinQuantity, MaxQuantity, UnitPriceHT).</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="201">Palier créé — retourne l'identifiant du palier.</response>
    /// <response code="400">Données invalides ou palier en conflit avec un existant.</response>
    [HttpPost("{id:guid}/variants/{variantId:guid}/price-tiers")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPriceTier(
        Guid id,
        Guid variantId,
        [FromBody] AddPriceTierBody body,
        CancellationToken ct = default)
    {
        var tierId = await mediator.Send(new AddPriceTierCommand
        {
            ProductId   = id,
            VariantId   = variantId,
            MinQuantity = body.MinQuantity,
            MaxQuantity = body.MaxQuantity,
            UnitPriceHT = body.UnitPriceHT
        }, ct);

        return Created($"api/v1/admin/products/{id}/variants/{variantId}/price-tiers/{tierId}", tierId);
    }

    // ── Endpoint d'upload d'image ────────────────────────────────────────────

    /// <summary>
    /// Upload une image pour un produit. Génère automatiquement une miniature WebP.
    /// </summary>
    /// <param name="id">Identifiant du produit.</param>
    /// <param name="file">Fichier image (jpg, jpeg, png, webp — 5 Mo max).</param>
    /// <param name="setAsMain">Si <c>true</c>, définit cette image comme image principale. Défaut : <c>true</c>.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="200">Image uploadée — retourne les URLs CDN principale et miniature.</response>
    /// <response code="400">Fichier manquant, format non supporté ou taille dépassée.</response>
    /// <response code="404">Produit introuvable.</response>
    [HttpPost("{id:guid}/image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadProductImageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(
        Guid id,
        IFormFile? file,
        [FromQuery] bool setAsMain = true,
        CancellationToken ct = default)
    {
        // ── Validation du fichier ────────────────────────────────────────────
        if (file is null)
            return BadRequest(new ApiErrorResponse(
                "FILE_REQUIRED", "Un fichier image est requis.", null, HttpContext.TraceIdentifier));

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new ApiErrorResponse(
                "FILE_TOO_LARGE", "Le fichier dépasse la limite de 5 Mo.", null, HttpContext.TraceIdentifier));

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(extension))
            return BadRequest(new ApiErrorResponse(
                "INVALID_FORMAT",
                "Format non supporté. Formats acceptés : jpg, jpeg, png, webp.",
                null,
                HttpContext.TraceIdentifier));

        // ── Dispatch de la commande ──────────────────────────────────────────
        await using var stream = file.OpenReadStream();

        var result = await mediator.Send(new UploadProductImageCommand
        {
            ProductId   = id,
            FileStream  = stream,
            FileName    = file.FileName,
            ContentType = file.ContentType,
            SetAsMain   = setAsMain
        }, ct);

        return Ok(result);
    }
}
