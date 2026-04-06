using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Phoenix.Api.Models.Auth;
using Phoenix.Application.Customers.Dtos;
using Phoenix.Domain.Products.ValueObjects;
using Phoenix.IntegrationTests.Common;

namespace Phoenix.IntegrationTests.Customers;

/// <summary>
/// Tests d'intégration des endpoints <c>/api/v1/customer</c>.
/// Vérifie les frontières 401/403 et les flux authentifiés de bout en bout.
/// </summary>
[Collection("Integration")]
public sealed class CustomerControllerTests : IClassFixture<PhoenixWebAppFactory>, IAsyncLifetime
{
    private readonly PhoenixWebAppFactory _factory;
    private readonly HttpClient           _anonymous;

    public CustomerControllerTests(PhoenixWebAppFactory factory)
    {
        _factory   = factory;
        _anonymous = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var fixture = new DatabaseFixture(_factory);
        await fixture.InitializeAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── GET /api/v1/customer/profile ──────────────────────────────────────────

    [Fact]
    public async Task GetProfile_WithoutToken_Returns401()
    {
        var response = await _anonymous.GetAsync("/api/v1/customer/profile");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_WithCustomerRole_Returns200OrNotFound()
    {
        var client = _factory.CreateAuthenticatedClient("Customer");

        var response = await client.GetAsync("/api/v1/customer/profile");

        // 200 OK (profile exists) or 404 NotFound (test user has no Customer profile)
        // Both are valid authenticated responses — not 401/403
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProfile_WithAdminRole_Returns200OrNotFound()
    {
        var client = _factory.CreateAuthenticatedClient("Admin");

        var response = await client.GetAsync("/api/v1/customer/profile");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound);
    }

    // ── PUT /api/v1/customer/profile ──────────────────────────────────────────

    [Fact]
    public async Task UpdateProfile_WithoutToken_Returns401()
    {
        var model = new
        {
            firstName   = "Alice",
            lastName    = "Dupont",
            companyName = (string?)null,
            segment     = "FastFood"
        };

        var response = await _anonymous.PutAsJsonAsync("/api/v1/customer/profile", model);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidPayload_Returns400()
    {
        var client = _factory.CreateAuthenticatedClient("Customer");
        var model  = new { firstName = "", lastName = "" }; // empty required fields

        var response = await client.PutAsJsonAsync("/api/v1/customer/profile", model);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── GET /api/v1/customer/dashboard ────────────────────────────────────────

    [Fact]
    public async Task GetDashboard_WithoutToken_Returns401()
    {
        var response = await _anonymous.GetAsync("/api/v1/customer/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDashboard_WithCustomerRole_Returns200OrNotFound()
    {
        var client = _factory.CreateAuthenticatedClient("Customer");

        var response = await client.GetAsync("/api/v1/customer/dashboard");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.NotFound);
    }

    // ── POST /api/v1/customer/addresses ──────────────────────────────────────

    [Fact]
    public async Task AddAddress_WithoutToken_Returns401()
    {
        var model = new
        {
            label      = "Mon restaurant",
            street     = "12 rue de la Paix",
            city       = "Paris",
            postalCode = "75001",
            country    = "FR"
        };

        var response = await _anonymous.PostAsJsonAsync("/api/v1/customer/addresses", model);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddAddress_WithInvalidPayload_Returns400()
    {
        var client = _factory.CreateAuthenticatedClient("Customer");
        var model  = new { label = "", street = "", city = "", postalCode = "" }; // all empty

        var response = await client.PostAsJsonAsync("/api/v1/customer/addresses", model);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── PUT /api/v1/customer/addresses/{id}/default ────────────────────────────

    [Fact]
    public async Task SetDefaultAddress_WithoutToken_Returns401()
    {
        var response = await _anonymous.PutAsync(
            $"/api/v1/customer/addresses/{Guid.NewGuid()}/default", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SetDefaultAddress_WithUnknownId_Returns404OrNotFound()
    {
        var client = _factory.CreateAuthenticatedClient("Customer");

        var response = await client.PutAsync(
            $"/api/v1/customer/addresses/{Guid.NewGuid()}/default", null);

        // Not 401/403 — the request was authenticated; 404 because customer/address not found
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest);
    }

    // ── Full register → login → get profile smoke test ─────────────────────────

    [Fact]
    public async Task Register_ThenGetProfile_WithRealJwt_Returns200()
    {
        // 1. Register new customer
        var email = $"smoke_{Guid.NewGuid():N}@restaurant.fr";
        var registerModel = new RegisterRequestModel(
            Email:           email,
            Password:        "Password1!",
            ConfirmPassword: "Password1!",
            FirstName:       "Smoke",
            LastName:        "Test",
            CompanyName:     null,
            Segment:         CustomerSegment.Other);

        var registerResponse = await _anonymous.PostAsJsonAsync("/api/v1/auth/register", registerModel);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var registerBody = await registerResponse.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        registerBody.Should().NotBeNull();

        // 2. Call /customer/profile with real JWT from registration
        var profileClient = _factory.CreateClient();
        profileClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", registerBody!.AccessToken);

        var profileResponse = await profileClient.GetAsync("/api/v1/customer/profile");

        // Should be 200 OK since customer was created during registration
        profileResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var profile = await profileResponse.Content
            .ReadFromJsonAsync<CustomerProfileDto>();
        profile.Should().NotBeNull();
        profile!.Email.Should().Be(email);
        profile.FirstName.Should().Be("Smoke");
    }

}
