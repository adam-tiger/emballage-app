using MediatR;
using Phoenix.Application.Products.Dtos;

namespace Phoenix.Application.Products.Queries.GetProductById;

/// <summary>
/// Query de récupération des détails complets d'un produit.
/// </summary>
public sealed record GetProductByIdQuery : IRequest<ProductDetailDto>
{
    /// <summary>Identifiant du produit à récupérer.</summary>
    public required Guid Id { get; init; }
}
