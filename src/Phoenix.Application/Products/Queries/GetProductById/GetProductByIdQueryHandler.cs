using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Products.Dtos;
using Phoenix.Application.Products.Mappings;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Application.Products.Queries.GetProductById;

/// <summary>
/// Gère la <see cref="GetProductByIdQuery"/> : charge le produit et le mappe
/// en <see cref="ProductDetailDto"/>. Lève une <see cref="NotFoundException"/> si absent.
/// </summary>
public sealed class GetProductByIdQueryHandler(
    IProductRepository repository,
    ProductMapper mapper)
    : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    /// <inheritdoc/>
    public async Task<ProductDetailDto> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), query.Id);

        return mapper.ToDetailDto(product);
    }
}
