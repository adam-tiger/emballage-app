using FluentAssertions;
using Moq;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Products.Mappings;
using Phoenix.Application.Products.Queries.GetProductById;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Products.Application;

/// <summary>
/// Tests unitaires du handler <see cref="GetProductByIdQueryHandler"/>.
/// </summary>
public sealed class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductRepository>    _repositoryMock = new();
    private readonly ProductMapper               _mapper         = new();
    private readonly GetProductByIdQueryHandler  _handler;
    private readonly CancellationToken           _ct = CancellationToken.None;

    public GetProductByIdQueryHandlerTests()
    {
        _handler = new GetProductByIdQueryHandler(_repositoryMock.Object, _mapper);
    }

    private static Product CreateProduct(string sku = "SAC-DETAIL-01") =>
        Product.Create(
            sku, "Sac Kraft Détail",
            ProductFamily.KraftBagHandled,
            true, false, false, false, true, false, true);

    // ── Cas nominal ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithExistingId_ReturnsProductDetailDto()
    {
        // Arrange
        var product = CreateProduct();
        var query = new GetProductByIdQuery { Id = product.Id };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, _ct))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Sku.Should().Be("SAC-DETAIL-01");
        result.NameFr.Should().Be("Sac Kraft Détail");
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldMapAllFlagsCorrectly()
    {
        // Arrange
        var product = CreateProduct();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, _ct))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(new GetProductByIdQuery { Id = product.Id }, _ct);

        // Assert
        result.IsCustomizable.Should().BeTrue();
        result.IsFoodApproved.Should().BeTrue();
        result.HasExpressDelivery.Should().BeTrue();
        result.IsGourmetRange.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldReturnEmptyCollections_WhenNoVariantsOrImages()
    {
        // Arrange
        var product = CreateProduct();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, _ct))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(new GetProductByIdQuery { Id = product.Id }, _ct);

        // Assert
        result.Variants.Should().NotBeNull();
        result.Variants.Should().BeEmpty();
        result.Images.Should().NotBeNull();
        result.Images.Should().BeEmpty();
    }

    // ── Produit introuvable ──────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithNonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var id    = Guid.CreateVersion7();
        var query = new GetProductByIdQuery { Id = id };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, _ct))
            .ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = () => _handler.Handle(query, _ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage("*Product*");
    }

    [Fact]
    public async Task Handle_WithNonExistingId_ShouldNeverCallMapper()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), _ct))
            .ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = () => _handler.Handle(
            new GetProductByIdQuery { Id = Guid.NewGuid() }, _ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();

        // Le repository a bien été interrogé exactement une fois
        _repositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), _ct),
            Times.Once);
    }
}
