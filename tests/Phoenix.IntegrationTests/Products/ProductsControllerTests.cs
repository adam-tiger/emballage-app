using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Phoenix.Application.Products.Dtos;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.IntegrationTests.Common;

namespace Phoenix.IntegrationTests.Products;

/// <summary>
/// Tests d'intégration des endpoints publics <c>/api/v1/products</c>.
/// Utilise le container PostgreSQL via <see cref="PhoenixWebAppFactory"/>.
/// </summary>
[Collection("Integration")]
public sealed class ProductsControllerTests : IClassFixture<PhoenixWebAppFactory>, IAsyncLifetime
{
    private readonly PhoenixWebAppFactory _factory;
    private readonly HttpClient           _client;

    public ProductsControllerTests(PhoenixWebAppFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var fixture = new DatabaseFixture(_factory);
        await fixture.InitializeAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── GET /api/v1/products ─────────────────────────────────────────────────

    [Fact]
    public async Task GetProducts_ReturnsOk_WithPaginatedList()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<PagedResult<ProductSummaryDto>>();

        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Page.Should().Be(1);
        result.TotalPages.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetProducts_WithIsActiveFilter_ReturnsOnlyActiveProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products?isActive=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<PagedResult<ProductSummaryDto>>();

        result!.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task GetProducts_WithPagination_ReturnsCorrectPage()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products?page=1&pageSize=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<PagedResult<ProductSummaryDto>>();

        result.Should().NotBeNull();
        result!.Items.Count.Should().BeLessOrEqualTo(3);
        result.PageSize.Should().Be(3);
    }

    [Fact]
    public async Task GetProducts_WithFamilyFilter_ReturnsFilteredProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products?family=GourmetRange");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<PagedResult<ProductSummaryDto>>();

        result.Should().NotBeNull();

        if (result!.Items.Any())
        {
            result.Items.Should().AllSatisfy(p =>
                p.Family.Should().Be("GourmetRange"));
        }
    }

    // ── GET /api/v1/products/{id} ─────────────────────────────────────────────

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsProductDetail()
    {
        // Arrange — récupérer d'abord la liste pour obtenir un vrai ID
        var listResponse = await _client.GetAsync("/api/v1/products");
        var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<ProductSummaryDto>>();
        var firstProduct = list!.Items.First();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{firstProduct.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<ProductDetailDto>();

        result.Should().NotBeNull();
        result!.Id.Should().Be(firstProduct.Id);
        result.Sku.Should().Be(firstProduct.Sku);
        result.Variants.Should().NotBeNull();
        result.Images.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_Returns404()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── GET /api/v1/products/families ────────────────────────────────────────

    [Fact]
    public async Task GetProductFamilies_ReturnsAllFamilies()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/families");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<List<ProductFamilyDto>>();

        result.Should().NotBeNull();
        result!.Should().HaveCount(27);
        result.Should().Contain(f => f.Value == "GourmetRange");
        result.Should().Contain(f => f.Value == "KraftBagHandled");
    }

    // ── GET /api/v1/products/sku/{sku} ───────────────────────────────────────

    [Fact]
    public async Task GetProductBySku_WithValidSku_ReturnsProduct()
    {
        // Arrange — récupérer la liste pour avoir un SKU valide
        var listResponse = await _client.GetAsync("/api/v1/products");
        var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<ProductSummaryDto>>();
        var sku = list!.Items.First().Sku;

        // Act
        var response = await _client.GetAsync($"/api/v1/products/sku/{sku}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ProductDetailDto>();
        result.Should().NotBeNull();
        result!.Sku.Should().Be(sku);
    }

    [Fact]
    public async Task GetProductBySku_WithInvalidSku_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/sku/SKU-INEXISTANT-999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
