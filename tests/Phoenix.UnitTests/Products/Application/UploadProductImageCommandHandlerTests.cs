using FluentAssertions;
using Moq;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Products.Commands.UploadProductImage;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Products.Application;

/// <summary>
/// Tests unitaires du handler <see cref="UploadProductImageCommandHandler"/>.
/// </summary>
public sealed class UploadProductImageCommandHandlerTests
{
    private readonly Mock<IProductRepository>             _repositoryMock    = new();
    private readonly Mock<IBlobStorageService>            _blobServiceMock   = new();
    private readonly Mock<IUnitOfWork>                    _unitOfWorkMock    = new();
    private readonly UploadProductImageCommandHandler     _handler;
    private readonly CancellationToken                    _ct = CancellationToken.None;

    public UploadProductImageCommandHandlerTests()
    {
        _handler = new UploadProductImageCommandHandler(
            _repositoryMock.Object,
            _blobServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static Product CreateProduct() =>
        Product.Create(
            "SAC-IMG-01", "Sac Image Test",
            ProductFamily.KraftBagHandled,
            false, false, false, false, false, false, false);

    private static UploadProductImageCommand BuildCommand(Guid productId) =>
        new()
        {
            ProductId   = productId,
            FileStream  = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF }),
            FileName    = "product-front.jpg",
            ContentType = "image/jpeg",
            SetAsMain   = true
        };

    private static BlobUploadResult BuildBlobResult(Guid productId) =>
        new(
            BlobPath:      $"products/{productId}/main.webp",
            PublicUrl:     $"https://cdn.phoenix.fr/products/{productId}/main.webp",
            ThumbBlobPath: $"products/{productId}/thumb.webp",
            ThumbPublicUrl: $"https://cdn.phoenix.fr/products/{productId}/thumb.webp");

    // ── Cas nominal ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithValidImage_UploadsToBlobAndUpdatesProduct()
    {
        // Arrange
        var product    = CreateProduct();
        var command    = BuildCommand(product.Id);
        var blobResult = BuildBlobResult(product.Id);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, _ct))
            .ReturnsAsync(product);

        _blobServiceMock
            .Setup(b => b.UploadProductImageAsync(
                product.Id, command.FileStream, command.FileName, _ct))
            .ReturnsAsync(blobResult);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Product>(), _ct))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(_ct))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.Should().NotBeNull();
        result.MainUrl.Should().NotBeNullOrEmpty();
        result.ProductId.Should().Be(product.Id);

        _blobServiceMock.Verify(
            b => b.UploadProductImageAsync(product.Id, command.FileStream, command.FileName, _ct),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(_ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidImage_ShouldAddImageToProduct()
    {
        // Arrange
        var product    = CreateProduct();
        var command    = BuildCommand(product.Id);
        var blobResult = BuildBlobResult(product.Id);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, _ct))
            .ReturnsAsync(product);

        _blobServiceMock
            .Setup(b => b.UploadProductImageAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>(), _ct))
            .ReturnsAsync(blobResult);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Product>(), _ct))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(_ct))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, _ct);

        // Assert — l'image a bien été ajoutée à l'agrégat
        product.Images.Should().ContainSingle();
        product.Images.First().IsMain.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithValidImage_ShouldReturnCorrectUrls()
    {
        // Arrange
        var product    = CreateProduct();
        var command    = BuildCommand(product.Id);
        var blobResult = BuildBlobResult(product.Id);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, _ct))
            .ReturnsAsync(product);

        _blobServiceMock
            .Setup(b => b.UploadProductImageAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>(), _ct))
            .ReturnsAsync(blobResult);

        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>(), _ct)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(_ct)).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.MainUrl.Should().Be(blobResult.PublicUrl);
        result.ThumbUrl.Should().Be(blobResult.ThumbPublicUrl);
    }

    // ── Produit introuvable ──────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithNonExistingProduct_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.CreateVersion7();
        var command = BuildCommand(nonExistingId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(nonExistingId, _ct))
            .ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = () => _handler.Handle(command, _ct);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage("*Product*");

        _blobServiceMock.Verify(
            b => b.UploadProductImageAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>(), _ct),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(_ct),
            Times.Never);
    }
}
