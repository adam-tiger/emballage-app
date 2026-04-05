using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Phoenix.Api.Models;
using Phoenix.IntegrationTests.Common;

namespace Phoenix.IntegrationTests.Products;

/// <summary>
/// Tests d'intégration des endpoints admin <c>/api/v1/admin/products</c>.
/// Couvre les scénarios d'authentification et d'autorisation par rôle.
/// </summary>
[Collection("Integration")]
public sealed class AdminProductsControllerTests : IClassFixture<PhoenixWebAppFactory>, IAsyncLifetime
{
    private readonly PhoenixWebAppFactory _factory;

    public AdminProductsControllerTests(PhoenixWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        var fixture = new DatabaseFixture(_factory);
        await fixture.InitializeAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private record CreateProductBody(
        string Sku,
        string NameFr,
        string DescriptionFr,
        string Family,
        bool IsCustomizable,
        bool IsGourmetRange,
        bool IsBulkOnly,
        bool IsEcoFriendly,
        bool IsFoodApproved,
        bool SoldByWeight,
        bool HasExpressDelivery);

    private static CreateProductBody BuildValidBody(string sku = "TEST-PROD-INTEGRATION-01") =>
        new(
            Sku:              sku,
            NameFr:           "Produit Test Intégration",
            DescriptionFr:    "Description pour test d'intégration",
            Family:           "KraftBagHandled",
            IsCustomizable:   true,
            IsGourmetRange:   false,
            IsBulkOnly:       false,
            IsEcoFriendly:    false,
            IsFoodApproved:   false,
            SoldByWeight:     false,
            HasExpressDelivery: false);

    // ── Création ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateProduct_AsAdmin_ReturnsCreated()
    {
        // Arrange
        var client  = _factory.CreateAuthenticatedClient("Admin");
        var body    = BuildValidBody($"TEST-CREATE-{Guid.NewGuid():N[..8]}");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/admin/products", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSku_Returns400()
    {
        // Arrange — utiliser le SKU "SAC-BRUN-22x10x28" du seed
        var client = _factory.CreateAuthenticatedClient("Admin");
        var body   = BuildValidBody("SAC-BRUN-22X10X28");

        // Act — premier appel pour créer
        await client.PostAsJsonAsync("/api/v1/admin/products", body);
        // Deuxième appel avec le même SKU
        var response = await client.PostAsJsonAsync("/api/v1/admin/products", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Code.Should().Be("VALIDATION_ERROR");
    }

    [Fact]
    public async Task CreateProduct_WithInvalidSku_Returns400()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("Admin");
        var body   = BuildValidBody("invalid sku with spaces");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/admin/products", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error!.Code.Should().Be("VALIDATION_ERROR");
    }

    [Fact]
    public async Task CreateProduct_WithoutAuth_Returns401()
    {
        // Arrange — client sans token d'authentification
        var client = _factory.CreateClient();
        var body   = BuildValidBody("TEST-UNAUTH-01");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/admin/products", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProduct_AsEmployee_Returns403()
    {
        // Arrange — Employee ne peut pas créer un produit (Admin uniquement)
        var client = _factory.CreateAuthenticatedClient("Employee");
        var body   = BuildValidBody("TEST-EMP-FORBIDDEN-01");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/admin/products", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ── Mise à jour ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateProduct_AsEmployee_ReturnsNoContent()
    {
        // Arrange — créer un produit via Admin
        var adminClient = _factory.CreateAuthenticatedClient("Admin");
        var createBody  = BuildValidBody($"TEST-UPD-{Guid.NewGuid():N[..8]}");
        var createResp  = await adminClient.PostAsJsonAsync("/api/v1/admin/products", createBody);
        var productId   = await createResp.Content.ReadFromJsonAsync<Guid>();

        var employeeClient = _factory.CreateAuthenticatedClient("Employee");
        var updateBody = new { NameFr = "Nom Mis à Jour", DescriptionFr = "Nouvelle description" };

        // Act
        var response = await employeeClient.PutAsJsonAsync(
            $"/api/v1/admin/products/{productId}", updateBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ── Désactivation ────────────────────────────────────────────────────────

    [Fact]
    public async Task DeactivateProduct_AsAdmin_ReturnsNoContent()
    {
        // Arrange — créer un produit à désactiver
        var adminClient = _factory.CreateAuthenticatedClient("Admin");
        var createBody  = BuildValidBody($"TEST-DEACT-{Guid.NewGuid():N[..8]}");
        var createResp  = await adminClient.PostAsJsonAsync("/api/v1/admin/products", createBody);
        var productId   = await createResp.Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await adminClient.DeleteAsync($"/api/v1/admin/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeactivateProduct_AsEmployee_Returns403()
    {
        // Arrange — créer un produit via Admin
        var adminClient = _factory.CreateAuthenticatedClient("Admin");
        var createBody  = BuildValidBody($"TEST-EMP-DEL-{Guid.NewGuid():N[..8]}");
        var createResp  = await adminClient.PostAsJsonAsync("/api/v1/admin/products", createBody);
        var productId   = await createResp.Content.ReadFromJsonAsync<Guid>();

        var employeeClient = _factory.CreateAuthenticatedClient("Employee");

        // Act — DELETE réservé aux Admins uniquement
        var response = await employeeClient.DeleteAsync($"/api/v1/admin/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeactivateProduct_WithNonExistingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("Admin");

        // Act
        var response = await client.DeleteAsync($"/api/v1/admin/products/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Admin list ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetProducts_AsAdmin_ReturnsAllProducts_IncludingInactive()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient("Admin");

        // Act
        var response = await client.GetAsync("/api/v1/admin/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
