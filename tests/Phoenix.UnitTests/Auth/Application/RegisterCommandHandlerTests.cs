using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using Phoenix.Application.Auth.Commands.Register;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Repositories;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Auth.Application;

/// <summary>
/// Tests unitaires du <see cref="RegisterCommandHandler"/>.
/// Vérifie le flux d'inscription : happy path, email dupliqué, échec Identity.
/// </summary>
public sealed class RegisterCommandHandlerTests
{
    // ── Mocks ─────────────────────────────────────────────────────────────────

    private readonly Mock<UserManager<ApplicationUser>>  _userManager;
    private readonly Mock<RoleManager<ApplicationRole>>  _roleManager;
    private readonly Mock<ICustomerRepository>           _customerRepo;
    private readonly Mock<IJwtTokenService>              _jwtService;
    private readonly Mock<IRefreshTokenStore>            _refreshTokenStore;
    private readonly Mock<IUnitOfWork>                   _unitOfWork;
    private readonly Mock<IEmailService>                 _emailService;
    private readonly RegisterCommandHandler              _handler;

    public RegisterCommandHandlerTests()
    {
        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        _roleManager = new Mock<RoleManager<ApplicationRole>>(
            Mock.Of<IRoleStore<ApplicationRole>>(),
            null!, null!, null!, null!);

        _customerRepo      = new Mock<ICustomerRepository>();
        _jwtService        = new Mock<IJwtTokenService>();
        _refreshTokenStore = new Mock<IRefreshTokenStore>();
        _unitOfWork        = new Mock<IUnitOfWork>();
        _emailService      = new Mock<IEmailService>();

        _handler = new RegisterCommandHandler(
            _userManager.Object,
            _roleManager.Object,
            _customerRepo.Object,
            _jwtService.Object,
            _refreshTokenStore.Object,
            _unitOfWork.Object,
            _emailService.Object);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static RegisterCommand BuildCommand(string email = "alice@restaurant.fr") => new()
    {
        Email           = email,
        Password        = "Password1!",
        ConfirmPassword = "Password1!",
        FirstName       = "Alice",
        LastName        = "Dupont",
        CompanyName     = null,
        Segment         = CustomerSegment.FastFood
    };

    private static TokenResult BuildTokenResult() => new(
        AccessToken:           "access-token",
        RefreshToken:          "refresh-token",
        AccessTokenExpiresAt:  DateTime.UtcNow.AddMinutes(15),
        RefreshTokenExpiresAt: DateTime.UtcNow.AddDays(7));

    private void SetupHappyPath()
    {
        // No existing user
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        // CreateAsync succeeds
        _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Role exists
        _roleManager.Setup(m => m.RoleExistsAsync("Customer"))
            .ReturnsAsync(true);

        // AddToRole succeeds
        _userManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"))
            .ReturnsAsync(IdentityResult.Success);

        // UpdateAsync after setting CustomerId
        _userManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // GetRolesAsync
        _userManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Customer"]);

        // JWT tokens
        _jwtService.Setup(s => s.GenerateTokens(It.IsAny<UserClaims>()))
            .Returns(BuildTokenResult());

        // Store refresh token
        _refreshTokenStore.Setup(s => s.StoreAsync(It.IsAny<RefreshTokenData>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // SaveChanges
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // AddAsync
        _customerRepo.Setup(r => r.AddAsync(It.IsAny<Phoenix.Domain.Customers.Entities.Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Email best-effort
        _emailService.Setup(e => e.SendWelcomeEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsAuthResponse()
    {
        SetupHappyPath();
        var command = BuildCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.User.Email.Should().Be("alice@restaurant.fr");
        result.User.FirstName.Should().Be("Alice");
        result.User.LastName.Should().Be("Dupont");
    }

    [Fact]
    public async Task Handle_WithValidCommand_StoresRefreshToken()
    {
        SetupHappyPath();

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _refreshTokenStore.Verify(s =>
            s.StoreAsync(
                It.Is<RefreshTokenData>(d => d.Token == "refresh-token"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesCustomerInRepository()
    {
        SetupHappyPath();

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _customerRepo.Verify(r =>
            r.AddAsync(
                It.Is<Phoenix.Domain.Customers.Entities.Customer>(c =>
                    c.Email == "alice@restaurant.fr" &&
                    c.FirstName == "Alice"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AssignsCustomerRole()
    {
        SetupHappyPath();

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _userManager.Verify(m =>
            m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRoleNotExists_CreatesRoleFirst()
    {
        SetupHappyPath();
        _roleManager.Setup(m => m.RoleExistsAsync("Customer")).ReturnsAsync(false);
        _roleManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationRole>()))
            .ReturnsAsync(IdentityResult.Success);

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        _roleManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationRole>()), Times.Once);
    }

    // ── Duplicate email ───────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithExistingEmail_ThrowsValidationException()
    {
        _userManager.Setup(m => m.FindByEmailAsync("alice@restaurant.fr"))
            .ReturnsAsync(new ApplicationUser { Email = "alice@restaurant.fr" });

        var act = () => _handler.Handle(BuildCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.ContainsKey("Email"));
    }

    // ── Identity failure ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenIdentityCreateFails_ThrowsValidationException()
    {
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Code = "PasswordTooWeak", Description = "Mot de passe trop faible." }));

        var act = () => _handler.Handle(BuildCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Values
                .SelectMany(v => v)
                .Any(msg => msg.Contains("Mot de passe trop faible")));
    }
}
