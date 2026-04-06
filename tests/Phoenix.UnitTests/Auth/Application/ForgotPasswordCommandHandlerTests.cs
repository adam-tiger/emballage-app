using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using Phoenix.Application.Auth.Commands.ForgotPassword;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.UnitTests.Auth.Application;

/// <summary>
/// Tests unitaires du <see cref="ForgotPasswordCommandHandler"/>.
/// Vérifie la propriété de sécurité : retourne toujours <see cref="Unit.Value"/>
/// sans révéler l'existence ou non d'un compte.
/// </summary>
public sealed class ForgotPasswordCommandHandlerTests
{
    // ── Mocks ─────────────────────────────────────────────────────────────────

    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<IEmailService>                _emailService;
    private readonly ForgotPasswordCommandHandler       _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        _emailService = new Mock<IEmailService>();

        _handler = new ForgotPasswordCommandHandler(
            _userManager.Object,
            _emailService.Object);
    }

    // ── Security: always Unit ─────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithExistingConfirmedEmail_ReturnsUnit()
    {
        var user = new ApplicationUser
        {
            Email          = "alice@restaurant.fr",
            UserName       = "alice@restaurant.fr",
            EmailConfirmed = true
        };

        _userManager.Setup(m => m.FindByEmailAsync("alice@restaurant.fr"))
            .ReturnsAsync(user);
        _userManager.Setup(m => m.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("reset-token-abc");
        _emailService.Setup(e => e.SendPasswordResetEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new ForgotPasswordCommand { Email = "alice@restaurant.fr" }, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_WithNonExistingEmail_ReturnsUnit()
    {
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _handler.Handle(
            new ForgotPasswordCommand { Email = "nobody@nowhere.fr" }, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_WithUnconfirmedEmail_ReturnsUnit()
    {
        var user = new ApplicationUser
        {
            Email          = "unconfirmed@test.fr",
            UserName       = "unconfirmed@test.fr",
            EmailConfirmed = false
        };

        _userManager.Setup(m => m.FindByEmailAsync("unconfirmed@test.fr"))
            .ReturnsAsync(user);

        var result = await _handler.Handle(
            new ForgotPasswordCommand { Email = "unconfirmed@test.fr" }, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }

    // ── Email sent only when appropriate ─────────────────────────────────────

    [Fact]
    public async Task Handle_WithExistingConfirmedEmail_SendsPasswordResetEmail()
    {
        var user = new ApplicationUser
        {
            Email          = "alice@restaurant.fr",
            UserName       = "alice@restaurant.fr",
            EmailConfirmed = true
        };

        _userManager.Setup(m => m.FindByEmailAsync("alice@restaurant.fr"))
            .ReturnsAsync(user);
        _userManager.Setup(m => m.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("reset-token-abc");
        _emailService.Setup(e => e.SendPasswordResetEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(
            new ForgotPasswordCommand { Email = "alice@restaurant.fr" }, CancellationToken.None);

        _emailService.Verify(e =>
            e.SendPasswordResetEmailAsync(
                "alice@restaurant.fr",
                It.Is<string>(link => link.Contains("reset-password")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingEmail_DoesNotSendEmail()
    {
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        await _handler.Handle(
            new ForgotPasswordCommand { Email = "nobody@nowhere.fr" }, CancellationToken.None);

        _emailService.Verify(e =>
            e.SendPasswordResetEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithUnconfirmedEmail_DoesNotSendEmail()
    {
        var user = new ApplicationUser
        {
            Email          = "unconfirmed@test.fr",
            UserName       = "unconfirmed@test.fr",
            EmailConfirmed = false
        };

        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        await _handler.Handle(
            new ForgotPasswordCommand { Email = "unconfirmed@test.fr" }, CancellationToken.None);

        _emailService.Verify(e =>
            e.SendPasswordResetEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
