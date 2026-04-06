using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Phoenix.Application.Auth.Commands.Login;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Auth.Application;

/// <summary>
/// Tests unitaires du <see cref="LoginCommandHandler"/>.
/// Vérifie : credentials valides, mot de passe erroné, compte verrouillé.
/// </summary>
public sealed class LoginCommandHandlerTests
{
    // ── Mocks ─────────────────────────────────────────────────────────────────

    private readonly Mock<UserManager<ApplicationUser>>   _userManager;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
    private readonly Mock<IJwtTokenService>               _jwtService;
    private readonly Mock<IRefreshTokenStore>             _refreshTokenStore;
    private readonly LoginCommandHandler                  _handler;

    public LoginCommandHandlerTests()
    {
        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        _signInManager = new Mock<SignInManager<ApplicationUser>>(
            _userManager.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null!, null!, null!, null!);

        _jwtService        = new Mock<IJwtTokenService>();
        _refreshTokenStore = new Mock<IRefreshTokenStore>();

        _handler = new LoginCommandHandler(
            _userManager.Object,
            _signInManager.Object,
            _jwtService.Object,
            _refreshTokenStore.Object);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static LoginCommand BuildCommand(
        string email    = "alice@restaurant.fr",
        string password = "Password1!") => new()
    {
        Email    = email,
        Password = password
    };

    private static ApplicationUser BuildUser() => new()
    {
        Id           = Guid.NewGuid().ToString(),
        Email        = "alice@restaurant.fr",
        UserName     = "alice@restaurant.fr",
        FirstName    = "Alice",
        LastName     = "Dupont",
        Segment      = CustomerSegment.FastFood,
        IsActive     = true,
        CreatedAtUtc = DateTime.UtcNow
    };

    private static TokenResult BuildTokenResult() => new(
        AccessToken:           "access-token",
        RefreshToken:          "refresh-token",
        AccessTokenExpiresAt:  DateTime.UtcNow.AddMinutes(15),
        RefreshTokenExpiresAt: DateTime.UtcNow.AddDays(7));

    private void SetupHappyPath(ApplicationUser user)
    {
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!))
            .ReturnsAsync(user);

        _signInManager.Setup(m => m.CheckPasswordSignInAsync(user, "Password1!", true))
            .ReturnsAsync(SignInResult.Success);

        _userManager.Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(["Customer"]);

        _jwtService.Setup(s => s.GenerateTokens(It.IsAny<UserClaims>()))
            .Returns(BuildTokenResult());

        _refreshTokenStore.Setup(s => s.StoreAsync(It.IsAny<RefreshTokenData>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsAuthResponse()
    {
        var user = BuildUser();
        SetupHappyPath(user);

        var result = await _handler.Handle(BuildCommand(), CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.User.Email.Should().Be("alice@restaurant.fr");
    }

    [Fact]
    public async Task Handle_WithValidCredentials_StoresRefreshToken()
    {
        var user = BuildUser();
        SetupHappyPath(user);

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _refreshTokenStore.Verify(s =>
            s.StoreAsync(
                It.Is<RefreshTokenData>(d => d.Token == "refresh-token" && d.UserId == user.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ── Unknown email ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithUnknownEmail_ThrowsValidationException()
    {
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var act = () => _handler.Handle(BuildCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Values
                .SelectMany(v => v)
                .Any(msg => msg.Contains("incorrect") || msg.Contains("Email")));
    }

    // ── Wrong password ────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsValidationException()
    {
        var user = BuildUser();
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _signInManager.Setup(m => m.CheckPasswordSignInAsync(user, It.IsAny<string>(), true))
            .ReturnsAsync(SignInResult.Failed);

        var act = () => _handler.Handle(
            BuildCommand(password: "WrongPassword!"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Values
                .SelectMany(v => v)
                .Any(msg => msg.Contains("incorrect")));
    }

    // ── Locked out ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithLockedOutAccount_ThrowsValidationExceptionWithLockoutMessage()
    {
        var user = BuildUser();
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _signInManager.Setup(m => m.CheckPasswordSignInAsync(user, It.IsAny<string>(), true))
            .ReturnsAsync(SignInResult.LockedOut);

        var act = () => _handler.Handle(BuildCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Values
                .SelectMany(v => v)
                .Any(msg => msg.Contains("bloqu") || msg.Contains("Compte")));
    }
}
