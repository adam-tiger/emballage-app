using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Phoenix.Api.Models.Auth;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Domain.Products.ValueObjects;
using Phoenix.IntegrationTests.Common;

// ReSharper disable StringLiteralTypo

namespace Phoenix.IntegrationTests.Auth;

/// <summary>
/// Tests d'intégration des endpoints <c>/api/v1/auth</c>.
/// Vérifie les frontières 401/400 et les flux register/login de bout en bout.
/// </summary>
[Collection("Integration")]
public sealed class AuthControllerTests : IClassFixture<PhoenixWebAppFactory>, IAsyncLifetime
{
    private readonly PhoenixWebAppFactory _factory;
    private readonly HttpClient           _client;

    public AuthControllerTests(PhoenixWebAppFactory factory)
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

    // ── POST /api/v1/auth/register ────────────────────────────────────────────

    [Fact]
    public async Task Register_WithValidData_Returns201WithAuthResponse()
    {
        var model = new RegisterRequestModel(
            Email:           $"test_{Guid.NewGuid():N}@restaurant.fr",
            Password:        "Password1!",
            ConfirmPassword: "Password1!",
            FirstName:       "Alice",
            LastName:        "Dupont",
            CompanyName:     null,
            Segment:         CustomerSegment.FastFood);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", model);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        body.Should().NotBeNull();
        body!.AccessToken.Should().NotBeNullOrEmpty();
        body.User.Email.Should().Contain("@restaurant.fr");
        body.User.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        var email = $"dup_{Guid.NewGuid():N}@restaurant.fr";
        var model = new RegisterRequestModel(
            Email:           email,
            Password:        "Password1!",
            ConfirmPassword: "Password1!",
            FirstName:       "Alice",
            LastName:        "Dupont",
            CompanyName:     null,
            Segment:         CustomerSegment.FastFood);

        // First registration succeeds
        await _client.PostAsJsonAsync("/api/v1/auth/register", model);

        // Second registration with same email fails
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", model);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithInvalidPayload_Returns400()
    {
        var model = new
        {
            email    = "not-an-email",
            password = "weak",
            // missing required fields
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", model);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── POST /api/v1/auth/login ───────────────────────────────────────────────

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithAccessToken()
    {
        // Register first
        var email = $"login_{Guid.NewGuid():N}@restaurant.fr";
        var registerModel = new RegisterRequestModel(
            Email:           email,
            Password:        "Password1!",
            ConfirmPassword: "Password1!",
            FirstName:       "Bob",
            LastName:        "Martin",
            CompanyName:     null,
            Segment:         CustomerSegment.PizzaShop);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerModel);

        // Then login
        var loginModel = new { email, password = "Password1!" };
        var response   = await _client.PostAsJsonAsync("/api/v1/auth/login", loginModel);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        body.Should().NotBeNull();
        body!.AccessToken.Should().NotBeNullOrEmpty();
        body.User.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns400()
    {
        var email = $"badpass_{Guid.NewGuid():N}@restaurant.fr";
        var registerModel = new RegisterRequestModel(
            Email:           email,
            Password:        "Password1!",
            ConfirmPassword: "Password1!",
            FirstName:       "Bob",
            LastName:        "Martin",
            CompanyName:     null,
            Segment:         CustomerSegment.Other);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerModel);

        var loginModel = new { email, password = "WrongPassword!" };
        var response   = await _client.PostAsJsonAsync("/api/v1/auth/login", loginModel);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns400()
    {
        var loginModel = new { email = "nobody@nowhere.fr", password = "Password1!" };
        var response   = await _client.PostAsJsonAsync("/api/v1/auth/login", loginModel);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── GET /api/v1/auth/me — requires authentication ─────────────────────────

    [Fact]
    public async Task GetMe_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_WithAuthenticatedClient_Returns200()
    {
        var authenticatedClient = _factory.CreateAuthenticatedClient("Customer");

        var response = await authenticatedClient.GetAsync("/api/v1/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── POST /api/v1/auth/logout — requires authentication ────────────────────

    [Fact]
    public async Task Logout_WithoutToken_Returns401()
    {
        var response = await _client.PostAsync("/api/v1/auth/logout", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithAuthenticatedClient_Returns204()
    {
        var authenticatedClient = _factory.CreateAuthenticatedClient("Customer");

        var response = await authenticatedClient.PostAsync("/api/v1/auth/logout", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ── POST /api/v1/auth/forgot-password — always 204 ───────────────────────

    [Fact]
    public async Task ForgotPassword_WithAnyEmail_Returns204()
    {
        var model = new { email = "nonexistent@example.fr" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", model);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ForgotPassword_WithExistingEmail_Returns204()
    {
        var email = $"forgot_{Guid.NewGuid():N}@restaurant.fr";
        var registerModel = new RegisterRequestModel(
            Email:           email,
            Password:        "Password1!",
            ConfirmPassword: "Password1!",
            FirstName:       "Alice",
            LastName:        "Dupont",
            CompanyName:     null,
            Segment:         CustomerSegment.Other);
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerModel);

        var forgotModel = new { email };
        var response    = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", forgotModel);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
