using MediatR;
using Phoenix.Application.Products.Dtos;
using Phoenix.Application.Products.Mappings;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Application.Products.Queries.GetProductList;

/// <summary>
/// Gère la <see cref="GetProductListQuery"/> : construit le filtre, délègue au repository,
/// mappe les entités en DTOs et retourne un <see cref="PagedResult{T}"/>.
/// </summary>
public sealed class GetProductListQueryHandler(
    IProductRepository repository,
    ProductMapper mapper)
    : IRequestHandler<GetProductListQuery, PagedResult<ProductSummaryDto>>
{
    /// <inheritdoc/>
    public async Task<PagedResult<ProductSummaryDto>> Handle(
        GetProductListQuery query,
        CancellationToken cancellationToken)
    {
        var filter = new ProductListFilter
        {
            Page          = query.Page,
            PageSize      = query.PageSize,
            SortBy        = query.SortBy,
            SortDir       = query.SortDir,
            Family        = query.Family,
            Segment       = query.Segment,
            IsCustomizable = query.IsCustomizable,
            SearchText    = query.SearchText,
            IsActive      = query.IsActive
        };

        var pagedDomain = await repository.GetListAsync(filter, cancellationToken);

        var dtos = pagedDomain.Items
            .Select(mapper.ToSummaryDto)
            .ToList();

        return new PagedResult<ProductSummaryDto>(
            dtos,
            pagedDomain.Page,
            pagedDomain.PageSize,
            pagedDomain.TotalCount);
    }
}
