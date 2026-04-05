using FluentAssertions;
using Moq;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Products.Commands.CreateProduct;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Products.Application;

/// <summary>
/// Tests unitaires du handler <see cref="CreateProductCommandHandler"/>.
/// </summary>
public sealed class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository>  _repositoryMock = new();
    private readonly Mock<IUnitOfWork>         _unitOfWorkMock = new();
    private readonly CreateProductCommandHandler _handler;
    private readonly CancellationToken          _ct = CancellationToken.None;

    public CreateProductCommandHandlerTests()
    {
        _handler = new CreateProductCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    private static CreateProductCommand BuildCommand(string sku = "SAC-BRUN-01") =>
        new()
        {
            Sku              = sku,
            NameFr           = "Sac Kraft Brun Test",
            DescriptionFr    = "Description test",
            Family           = ProductFamily.KraftBagHandled,
            IsCustomizable   = true,
            IsGourmetRange   = false,
            IsBulkOnly       = false,
            IsEcoFriendly    = false,
            IsFoodApproved   = false,
            SoldByWeight     = false,
            HasExpressDelivery = true
        };

    // ── Cas nominal ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProductAndReturnId()
    {
        // Arrange
        var command = BuildCommand("SAC-BRUN-01");

        _repositoryMock
            .Setup(r => r.ExistsAsync("SAC-BRUN-01", _ct))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), _ct))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(_ct))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.Should().NotBeEmpty();

        _repositoryMock.Verify(
            r => r.AddAsync(It.Is<Product>(p => p.Sku == "SAC-BRUN-01"), _ct),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(_ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProductWithCorrectFlags()
    {
        // Arrange
        var command = BuildCommand();
        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<string>(), _ct)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>(), _ct)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(_ct)).ReturnsAsync(1);

        Product? capturedProduct = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), _ct))
            .Callback<Product, CancellationToken>((p, _) => capturedProduct = p)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, _ct);

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct!.IsCustomizable.Should().BeTrue();
        capturedProduct.HasExpressDelivery.Should().BeTrue();
        capturedProduct.IsActive.Should().BeTrue();
        capturedProduct.Family.Should().Be(ProductFamily.KraftBagHandled);
    }

    // ── SKU dupliqué ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithDuplicateSku_ShouldThrowValidationException()
    {
        // Arrange
        var command = BuildCommand("SAC-BRUN-01");

        _repositoryMock
            .Setup(r => r.ExistsAsync("SAC-BRUN-01", _ct))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = () => _handler.Handle(command, _ct);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Product>(), _ct),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(_ct),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithDuplicateSku_ShouldIncludeSkuInValidationErrors()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<string>(), _ct)).ReturnsAsync(true);

        // Act
        Func<Task> act = () => _handler.Handle(BuildCommand(), _ct);

        // Assert
        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().ContainKey("Sku");
    }

    // ── Validation du validator ───────────────────────────────────────────────

    [Fact]
    public void Handle_WithInvalidSku_ValidatorShouldReturnErrors()
    {
        // Arrange
        var command = BuildCommand() with { Sku = "" };
        var validator = new CreateProductCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Sku");
    }

    [Fact]
    public void Handle_WithSkuContainingSpaces_ValidatorShouldReturnErrors()
    {
        // Arrange
        var command = BuildCommand() with { Sku = "invalid sku spaces" };
        var validator = new CreateProductCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Sku");
    }

    [Fact]
    public void Handle_WithLowercaseSku_ValidatorShouldReturnErrors()
    {
        // Arrange
        var command = BuildCommand() with { Sku = "sac-brun-01" };
        var validator = new CreateProductCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Sku");
    }

    [Fact]
    public void Handle_WithEmptyNameFr_ValidatorShouldReturnErrors()
    {
        // Arrange
        var command = BuildCommand() with { NameFr = "" };
        var validator = new CreateProductCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NameFr");
    }

    [Fact]
    public void Handle_WithValidCommand_ValidatorShouldPass()
    {
        // Arrange
        var command = BuildCommand("SAC-BRUN-01");
        var validator = new CreateProductCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
