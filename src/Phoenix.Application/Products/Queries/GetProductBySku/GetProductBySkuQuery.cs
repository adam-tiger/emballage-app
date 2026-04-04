using MediatR;
using Phoenix.Application.Products.Dtos;

namespace Phoenix.Application.Products.Queries.GetProductBySku;

/// <summary>
/// Query de récupération des détails complets d'un produit par son SKU.
/// </summary>
/// <param name="Sku">Référence SKU du produit (insensible à la casse).</param>
public sealed record GetProductBySkuQuery(string Sku) : IRequest<ProductDetailDto>;
