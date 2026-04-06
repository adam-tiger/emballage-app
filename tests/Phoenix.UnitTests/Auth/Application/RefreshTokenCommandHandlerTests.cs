using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Phoenix.Application.Auth.Commands.RefreshToken;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Auth.Application;

/// <summary>
/// Tests unitaires du <see cref="RefreshTokenCommandHandler"/>.
/// Vérifie : rotation de token, token révoqué, token expiré, token invalide.
/// </summary>
public sealed class RefreshTokenCommandHandlerTests
{
    // ── Mocks ─────────────────────────────────────────────────────────────────

    private readonly Mock<IRefreshTokenStore>            _refreshTokenStore;
    private readonly Mock<UserManager<ApplicationUser>>  _userManager;
    private readonly Mock<IJwtTokenService>              _jwtService;
    private readonly RefreshTokenCommandHandler          _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenStore = new Mock<IRefreshTokenStore>();

        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        _jwtService = new Mock<IJwtTokenService>();

        _handler = new RefreshTokenCommandHandler(
            _refreshTokenStore.Object,
            _userManager.Object,
            _jwtService.Object);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private const string ValidToken = "valid-refresh-token";
    private const string UserId     = "user-id-123";

    private static RefreshTokenData BuildTokenData(
        bool isRevoked     = false,
        DateTime? expiresAt = null) => new(
        Token:     ValidToken,
        UserId:    UserId,
        ExpiresAt: expiresAt ?? DateTime.UtcNow.AddDays(7),
        IsRevoked: isRevoked,
        CreatedAt: DateTime.UtcNow.AddHours(-1));

    private static ApplicationUser BuildUser() => new()
    {
        Id           = UserId,
        Email        = "alice@restaurant.fr",
        UserName     = "alice@restaurant.fr",
        FirstName    = "Alice",
        LastName     = "Dupont",
        Segment      = CustomerSegment.FastFood,
        IsActive     = true,
        CreatedAtUtc = DateTime.UtcNow
    };

    private static TokenResult BuildTokenResult() => new(
        AccessToken:           "new-access-token",
        RefreshToken:          "new-refresh-token",
        AccessTokenExpiresAt:  DateTime.UtcNow.AddMinutes(15),
        RefreshTokenExpiresAt: DateTime.UtcNow.AddDays(7));

    private void SetupHappyPath()
    {
        _refreshTokenStore.Setup(s => s.GetAsync(ValidToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildTokenData());

        _refreshTokenStore.Setup(s => s.RevokeAsync(ValidToken, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userManager.Setup(m => m.FindByIdAsync(UserId))
            .ReturnsAsync(BuildUser());

        _userManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Customer"]);

        _jwtService.Setup(s => s.GenerateTokens(It.IsAny<UserClaims>()))
            .Returns(BuildTokenResult());

        _refreshTokenStore.Setup(s => s.StoreAsync(It.IsAny<RefreshTokenData>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    // ── Happy path — token rotation ───────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidToken_ReturnsNewAuthResponse()
    {
        SetupHappyPath();
        var command = new RefreshTokenCommand { RefreshToken = ValidToken };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task Handle_WithValidToken_RevokesOldToken()
    {
        SetupHappyPath();

        await _handler.Handle(new RefreshTokenCommand { RefreshToken = ValidToken }, CancellationToken.None);

        _refreshTokenStore.Verify(s =>
            s.RevokeAsync(ValidToken, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidToken_StoresNewRefreshToken()
    {
        SetupHappyPath();

        await _handler.Handle(new RefreshTokenCommand { RefreshToken = ValidToken }, CancellationToken.None);

        _refreshTokenStore.Verify(s =>
            s.StoreAsync(
                It.Is<RefreshTokenData>(d => d.Token == "new-refresh-token"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ── Invalid token ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithUnknownToken_ThrowsValidationException()
    {
        _refreshTokenStore.Setup(s => s.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshTokenData?)null);

        var act = () => _handler.Handle(
            new RefreshTokenCommand { RefreshToken = "unknown-token" }, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Values
                .SelectMany(v => v)
                .Any(msg => msg.Contains("invalide") || msg.Contains("Token")));
    }

    // ── Revoked token ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithRevokedToken_ThrowsValidationException()
    {
        _refreshTokenStore.Setup(s => s.GetAsync(ValidToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildTokenData(isRevoked: true));

        var act = () => _handler.Handle(
            new RefreshTokenCommand { RefreshToken = ValidToken }, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Values
                .SelectMany(v => v)
                .Any(msg => msg.Contains("révoqué") || msg.Contains("Token")));
    }

    // ── Expired token ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithExpiredToken_ThrowsValidationException()
    {
        _refreshTokenStore.Setup(s => s.GetAsync(ValidToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildTokenData(expiresAt: DateTime.UtcNow.AddDays(-1)));

        var act = () => _handler.Handle(
            new RefreshTokenCommand { RefreshToken = ValidToken }, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Values
                .SelectMany(v => v)
                .Any(msg => msg.Contains("expiré") || msg.Contains("Token")));
    }
}
