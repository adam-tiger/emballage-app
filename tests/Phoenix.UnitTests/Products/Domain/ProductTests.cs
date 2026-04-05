using FluentAssertions;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.Events;
using Phoenix.Domain.Products.Exceptions;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Products.Domain;

/// <summary>
/// Tests unitaires de l'agrégat racine <see cref="Product"/>.
/// Aucune dépendance infrastructure — domaine pur.
/// </summary>
public sealed class ProductTests
{
    // ── Helpers ─────────────────────────────────────────────────────────────

    private static Product CreateValidProduct(string sku = "SAC-BRUN-01") =>
        Product.Create(
            sku,
            "Sac Kraft Brun",
            ProductFamily.KraftBagHandled,
            isCustomizable: true,
            isGourmetRange: false,
            isBulkOnly: false,
            isEcoFriendly: false,
            isFoodApproved: false,
            soldByWeight: false,
            hasExpressDelivery: true);

    private static ProductVariant CreateVariant(string sku = "VAR-01") =>
        new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            sku,
            "Variante test",
            minimumOrderQuantity: 250,
            PrintSide.SingleSide,
            ColorCount.One,
            DateTime.UtcNow);

    private static ProductImage CreateImage(bool isMain = false) =>
        new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "products/test/image.webp",
            "https://cdn.phoenix.fr/products/test/image.webp",
            isMain,
            DateTime.UtcNow);

    // ── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        // Act
        var product = CreateValidProduct("SAC-BRUN-01");

        // Assert
        product.Should().NotBeNull();
        product.Sku.Should().Be("SAC-BRUN-01");
        product.NameFr.Should().Be("Sac Kraft Brun");
        product.Family.Should().Be(ProductFamily.KraftBagHandled);
        product.IsCustomizable.Should().BeTrue();
        product.HasExpressDelivery.Should().BeTrue();
        product.IsActive.Should().BeTrue();
        product.DomainEvents.Should().ContainSingle();
        product.DomainEvents.First().Should().BeOfType<ProductCreatedEvent>();
    }

    [Fact]
    public void Create_ShouldNormalizeSku_ToUppercase()
    {
        // Act
        var product = Product.Create(
            "sac-brun-01", "Sac Brun",
            ProductFamily.KraftBagHandled,
            false, false, false, false, false, false, false);

        // Assert
        product.Sku.Should().Be("SAC-BRUN-01");
    }

    [Fact]
    public void Create_WithEmptySku_ShouldThrowProductDomainException()
    {
        // Arrange
        Action act = () => Product.Create(
            "",
            "Nom Produit",
            ProductFamily.KraftBagHandled,
            false, false, false, false, false, false, false);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.SkuRequired);
    }

    [Fact]
    public void Create_WithWhitespaceSku_ShouldThrowProductDomainException()
    {
        // Arrange
        Action act = () => Product.Create(
            "   ",
            "Nom Produit",
            ProductFamily.KraftBagHandled,
            false, false, false, false, false, false, false);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.SkuRequired);
    }

    [Fact]
    public void Create_ShouldInitializeWithEmptyCollections()
    {
        // Act
        var product = CreateValidProduct();

        // Assert
        product.Variants.Should().BeEmpty();
        product.Images.Should().BeEmpty();
    }

    // ── Deactivate ───────────────────────────────────────────────────────────

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse_AndRaiseDomainEvent()
    {
        // Arrange
        var product = CreateValidProduct();
        product.ClearDomainEvents();

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
        product.DomainEvents.OfType<ProductDeactivatedEvent>()
               .Should().ContainSingle();
    }

    [Fact]
    public void Deactivate_ShouldUpdateUpdatedAtUtc()
    {
        // Arrange
        var product = CreateValidProduct();
        var beforeDeactivate = product.UpdatedAtUtc;

        // Act
        product.Deactivate();

        // Assert
        product.UpdatedAtUtc.Should().BeOnOrAfter(beforeDeactivate);
    }

    // ── AddVariant ───────────────────────────────────────────────────────────

    [Fact]
    public void AddVariant_WithNewSku_ShouldAddVariant()
    {
        // Arrange
        var product = CreateValidProduct();
        var variant = CreateVariant("VAR-01");

        // Act
        product.AddVariant(variant);

        // Assert
        product.Variants.Should().ContainSingle();
        product.Variants.First().Sku.Should().Be("VAR-01");
    }

    [Fact]
    public void AddVariant_WithDuplicateSku_ShouldThrowProductDomainException()
    {
        // Arrange
        var product = CreateValidProduct();
        var variant1 = CreateVariant("VAR-01");
        var variant2 = CreateVariant("VAR-01");
        product.AddVariant(variant1);

        // Act
        Action act = () => product.AddVariant(variant2);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.SkuAlreadyExists);
    }

    [Fact]
    public void AddVariant_WithDuplicateSkuCaseInsensitive_ShouldThrowProductDomainException()
    {
        // Arrange
        var product = CreateValidProduct();
        product.AddVariant(CreateVariant("VAR-01"));

        Action act = () => product.AddVariant(
            new ProductVariant(
                Guid.CreateVersion7(), Guid.CreateVersion7(),
                "var-01", "Doublon minuscule", 250,
                PrintSide.SingleSide, ColorCount.One, DateTime.UtcNow));

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.SkuAlreadyExists);
    }

    // ── SetMainImage ─────────────────────────────────────────────────────────

    [Fact]
    public void SetMainImage_ShouldSetOnlyOneMainImage()
    {
        // Arrange
        var product = CreateValidProduct();
        var image1 = CreateImage(isMain: true);
        var image2 = CreateImage(isMain: false);
        product.AddImage(image1);
        product.AddImage(image2);

        // Act
        product.SetMainImage(image2.Id);

        // Assert
        product.Images.Count(i => i.IsMain).Should().Be(1);
        product.Images.First(i => i.IsMain).Id.Should().Be(image2.Id);
    }

    [Fact]
    public void SetMainImage_WithNonExistingImageId_ShouldThrowProductDomainException()
    {
        // Arrange
        var product = CreateValidProduct();
        product.AddImage(CreateImage(isMain: true));

        // Act
        Action act = () => product.SetMainImage(Guid.NewGuid());

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.InvalidImagePath);
    }

    [Fact]
    public void AddImage_FirstImage_ShouldBeSetAsMain_Automatically()
    {
        // Arrange
        var product = CreateValidProduct();
        var image = CreateImage(isMain: false);

        // Act
        product.AddImage(image);

        // Assert
        product.Images.Single().IsMain.Should().BeTrue();
    }

    // ── DomainEvents ─────────────────────────────────────────────────────────

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var product = CreateValidProduct();
        product.DomainEvents.Should().NotBeEmpty();

        // Act
        product.ClearDomainEvents();

        // Assert
        product.DomainEvents.Should().BeEmpty();
    }
}
