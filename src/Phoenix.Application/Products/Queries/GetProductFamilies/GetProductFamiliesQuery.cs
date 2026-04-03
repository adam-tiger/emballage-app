using MediatR;
using Phoenix.Application.Products.Dtos;

namespace Phoenix.Application.Products.Queries.GetProductFamilies;

/// <summary>
/// Query de récupération de la liste statique de toutes les familles produit avec leurs libellés français.
/// Aucun accès base de données — dérivé de l'enum <c>ProductFamily</c>.
/// </summary>
public sealed record GetProductFamiliesQuery : IRequest<IReadOnlyList<ProductFamilyDto>>;
