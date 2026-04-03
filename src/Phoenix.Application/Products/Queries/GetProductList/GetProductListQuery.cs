using MediatR;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Products.Queries.GetProductList;

/// <summary>
/// Query de récupération d'une liste paginée et filtrée de produits pour l'affichage catalogue.
/// </summary>
public sealed record GetProductListQuery : IRequest<PagedResult<Products.Dtos.ProductSummaryDto>>
{
    /// <summary>Numéro de page demandé (1-based). Défaut : 1.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Nombre d'éléments par page. Défaut : 20.</summary>
    public int PageSize { get; init; } = 20;

    /// <summary>Propriété sur laquelle trier (ex : "NameFr", "Sku"). Défaut : "NameFr".</summary>
    public string SortBy { get; init; } = "NameFr";

    /// <summary>Direction du tri : "asc" ou "desc". Défaut : "asc".</summary>
    public string SortDir { get; init; } = "asc";

    /// <summary>Filtre optionnel par famille de produits.</summary>
    public ProductFamily? Family { get; init; }

    /// <summary>Filtre optionnel par segment client cible.</summary>
    public CustomerSegment? Segment { get; init; }

    /// <summary>Filtre optionnel sur les produits personnalisables uniquement.</summary>
    public bool? IsCustomizable { get; init; }

    /// <summary>Recherche plein-texte sur Sku, NameFr et DescriptionFr.</summary>
    public string? SearchText { get; init; }

    /// <summary>Filtre par statut actif. <c>null</c> = tous. Défaut : <c>true</c> (actifs seulement).</summary>
    public bool? IsActive { get; init; } = true;
}
