using FluentAssertions;
using Moq;
using Phoenix.Application.Products.Mappings;
using Phoenix.Application.Products.Queries.GetProductList;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Products.Application;

/// <summary>
/// Tests unitaires du handler <see cref="GetProductListQueryHandler"/>.
/// </summary>
public sealed class GetProductListQueryHandlerTests
{
    private readonly Mock<IProductRepository>    _repositoryMock = new();
    private readonly ProductMapper               _mapper         = new();
    private readonly GetProductListQueryHandler  _handler;
    private readonly CancellationToken           _ct = CancellationToken.None;

    public GetProductListQueryHandlerTests()
    {
        _handler = new GetProductListQueryHandler(_repositoryMock.Object, _mapper);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static Product CreateActiveProduct(string sku, string name = "Produit Test") =>
        Product.Create(
            sku, name, ProductFamily.KraftBagHandled,
            false, false, false, false, false, false, false);

    private static PagedResult<Product> BuildPagedResult(IReadOnlyList<Product> items, int total = -1) =>
        new(items, 1, 20, total >= 0 ? total : items.Count);

    // ── Cas nominal ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ReturnsPagedResult_WithDefaultFilters()
    {
        // Arrange
        var products = new List<Product>
        {
            CreateActiveProduct("PROD-01", "Produit 1"),
            CreateActiveProduct("PROD-02", "Produit 2"),
            CreateActiveProduct("PROD-03", "Produit 3")
        };

        _repositoryMock
            .Setup(r => r.GetListAsync(It.IsAny<ProductListFilter>(), _ct))
            .ReturnsAsync(BuildPagedResult(products));

        var query = new GetProductListQuery { Page = 1, PageSize = 20 };

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldMapProductsToSummaryDtos()
    {
        // Arrange
        var products = new List<Product> { CreateActiveProduct("PROD-MAP-01", "Sac Kraft") };
        _repositoryMock
            .Setup(r => r.GetListAsync(It.IsAny<ProductListFilter>(), _ct))
            .ReturnsAsync(BuildPagedResult(products));

        // Act
        var result = await _handler.Handle(new GetProductListQuery(), _ct);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().Sku.Should().Be("PROD-MAP-01");
        result.Items.First().NameFr.Should().Be("Sac Kraft");
    }

    [Fact]
    public async Task Handle_WithIsCustomizableFilter_PassesFilterToRepository()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetListAsync(It.IsAny<ProductListFilter>(), _ct))
            .ReturnsAsync(BuildPagedResult([]));

        var query = new GetProductListQuery { IsCustomizable = true };

        // Act
        await _handler.Handle(query, _ct);

        // Assert
        _repositoryMock.Verify(
            r => r.GetListAsync(
                It.Is<ProductListFilter>(f => f.IsCustomizable == true),
                _ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithSearchText_PassesSearchToRepository()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetListAsync(It.IsAny<ProductListFilter>(), _ct))
            .ReturnsAsync(BuildPagedResult([]));

        var query = new GetProductListQuery { SearchText = "kraft" };

        // Act
        await _handler.Handle(query, _ct);

        // Assert
        _repositoryMock.Verify(
            r => r.GetListAsync(
                It.Is<ProductListFilter>(f => f.SearchText == "kraft"),
                _ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithFamilyFilter_PassesFilterToRepository()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetListAsync(It.IsAny<ProductListFilter>(), _ct))
            .ReturnsAsync(BuildPagedResult([]));

        var query = new GetProductListQuery { Family = ProductFamily.GourmetRange };

        // Act
        await _handler.Handle(query, _ct);

        // Assert
        _repositoryMock.Verify(
            r => r.GetListAsync(
                It.Is<ProductListFilter>(f => f.Family == ProductFamily.GourmetRange),
                _ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithPagination_PassesPaginationToRepository()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetListAsync(It.IsAny<ProductListFilter>(), _ct))
            .ReturnsAsync(new PagedResult<Product>([], 3, 10, 25));

        var query = new GetProductListQuery { Page = 3, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        _repositoryMock.Verify(
            r => r.GetListAsync(
                It.Is<ProductListFilter>(f => f.Page == 3 && f.PageSize == 10),
                _ct),
            Times.Once);

        result.Page.Should().Be(3);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithEmptyResult_ShouldReturnEmptyPage()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetListAsync(It.IsAny<ProductListFilter>(), _ct))
            .ReturnsAsync(BuildPagedResult([]));

        // Act
        var result = await _handler.Handle(new GetProductListQuery(), _ct);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}
