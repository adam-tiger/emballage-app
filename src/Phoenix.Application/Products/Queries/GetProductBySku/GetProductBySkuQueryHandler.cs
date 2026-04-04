using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Products.Dtos;
using Phoenix.Application.Products.Mappings;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Application.Products.Queries.GetProductBySku;

/// <summary>
/// Gère la <see cref="GetProductBySkuQuery"/> : charge le produit par SKU et le mappe
/// en <see cref="ProductDetailDto"/>. Lève une <see cref="NotFoundException"/> si absent.
/// </summary>
public sealed class GetProductBySkuQueryHandler(
    IProductRepository repository,
    ProductMapper mapper)
    : IRequestHandler<GetProductBySkuQuery, ProductDetailDto>
{
    /// <inheritdoc/>
    public async Task<ProductDetailDto> Handle(
        GetProductBySkuQuery query,
        CancellationToken cancellationToken)
    {
        var sku = query.Sku.Trim().ToUpperInvariant();

        var product = await repository.GetBySkuAsync(sku, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), sku);

        return mapper.ToDetailDto(product);
    }
}
