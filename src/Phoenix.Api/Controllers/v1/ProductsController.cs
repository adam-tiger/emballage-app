using MediatR;
using Microsoft.AspNetCore.Mvc;
using Phoenix.Api.Models;
using Phoenix.Application.Products.Dtos;
using Phoenix.Application.Products.Queries.GetProductById;
using Phoenix.Application.Products.Queries.GetProductBySku;
using Phoenix.Application.Products.Queries.GetProductFamilies;
using Phoenix.Application.Products.Queries.GetProductList;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Api.Controllers.v1;

/// <summary>
/// Endpoints publics du catalogue produit — aucune authentification requise.
/// </summary>
[ApiController]
[Route("api/v1/products")]
[Produces("application/json")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Retourne une liste paginée et filtrée des produits actifs du catalogue.
    /// </summary>
    /// <param name="page">Numéro de page (base 1). Défaut : 1.</param>
    /// <param name="pageSize">Nombre d'éléments par page. Défaut : 20.</param>
    /// <param name="sortBy">Propriété de tri (ex : "NameFr", "Sku"). Défaut : "NameFr".</param>
    /// <param name="sortDir">Direction du tri : "asc" ou "desc". Défaut : "asc".</param>
    /// <param name="family">Filtre optionnel par famille de produits.</param>
    /// <param name="segment">Filtre optionnel par segment client cible.</param>
    /// <param name="isCustomizable">Filtre optionnel sur les produits personnalisables.</param>
    /// <param name="searchText">Recherche plein-texte sur SKU, nom et description.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="200">Liste paginée des produits actifs.</response>
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
            IsActive       = true   // catalogue public : toujours actifs uniquement
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

    /// <summary>
    /// Retourne la liste des familles de produits disponibles dans le catalogue.
    /// </summary>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="200">Liste des 27 familles avec leur libellé français.</response>
    [HttpGet("families")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductFamilyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductFamilies(CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetProductFamiliesQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Retourne le détail complet d'un produit par son SKU.
    /// </summary>
    /// <param name="sku">Référence SKU du produit (insensible à la casse).</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <response code="200">Détail complet du produit.</response>
    /// <response code="404">Produit introuvable pour ce SKU.</response>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductBySku(string sku, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetProductBySkuQuery(sku), ct);
        return Ok(result);
    }
}
