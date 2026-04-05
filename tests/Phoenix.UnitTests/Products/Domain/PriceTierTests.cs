using FluentAssertions;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.Exceptions;

namespace Phoenix.UnitTests.Products.Domain;

/// <summary>
/// Tests unitaires de <see cref="PriceTier"/> :
/// validation des invariants et logique de correspondance quantité/palier.
/// </summary>
public sealed class PriceTierTests
{
    private static readonly Guid ValidId        = Guid.CreateVersion7();
    private static readonly Guid ValidVariantId = Guid.CreateVersion7();

    // ── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ShouldCreatePriceTier()
    {
        // Act
        var tier = new PriceTier(ValidId, ValidVariantId, 100, 499, 0.0872m);

        // Assert
        tier.MinQuantity.Should().Be(100);
        tier.MaxQuantity.Should().Be(499);
        tier.UnitPriceHT.Should().Be(0.0872m);
    }

    [Fact]
    public void Create_WithNullMax_ShouldCreateOpenEndedTier()
    {
        // Act
        var tier = new PriceTier(ValidId, ValidVariantId, 500, null, 0.0700m);

        // Assert
        tier.MaxQuantity.Should().BeNull();
    }

    [Fact]
    public void Create_WithMaxLessThanMin_ShouldThrowProductDomainException()
    {
        // Act
        Action act = () => new PriceTier(ValidId, ValidVariantId, 500, 499, 0.10m);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.InvalidPriceTier);
    }

    [Fact]
    public void Create_WithMaxEqualToMin_ShouldThrowProductDomainException()
    {
        // Act
        Action act = () => new PriceTier(ValidId, ValidVariantId, 250, 250, 0.10m);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.InvalidPriceTier);
    }

    [Fact]
    public void Create_WithZeroPrice_ShouldThrowProductDomainException()
    {
        // Act
        Action act = () => new PriceTier(ValidId, ValidVariantId, 100, 499, 0m);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.InvalidPriceTier);
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrowProductDomainException()
    {
        // Act
        Action act = () => new PriceTier(ValidId, ValidVariantId, 100, 499, -0.10m);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.InvalidPriceTier);
    }

    [Fact]
    public void Create_WithMinQuantityZero_ShouldThrowProductDomainException()
    {
        // Act
        Action act = () => new PriceTier(ValidId, ValidVariantId, 0, null, 0.10m);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.InvalidPriceTier);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new PriceTier(Guid.Empty, ValidVariantId, 100, null, 0.10m);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithParameterName("id");
    }

    // ── Matches ──────────────────────────────────────────────────────────────

    [Fact]
    public void Matches_QuantityInRange_ShouldReturnTrue()
    {
        // Arrange
        var tier = new PriceTier(ValidId, ValidVariantId, 250, 499, 0.0872m);

        // Act & Assert
        tier.Matches(250).Should().BeTrue();
        tier.Matches(300).Should().BeTrue();
        tier.Matches(499).Should().BeTrue();
    }

    [Fact]
    public void Matches_QuantityBelowMin_ShouldReturnFalse()
    {
        // Arrange
        var tier = new PriceTier(ValidId, ValidVariantId, 250, 499, 0.0872m);

        // Act & Assert
        tier.Matches(249).Should().BeFalse();
        tier.Matches(1).Should().BeFalse();
    }

    [Fact]
    public void Matches_QuantityAboveMax_ShouldReturnFalse()
    {
        // Arrange
        var tier = new PriceTier(ValidId, ValidVariantId, 250, 499, 0.0872m);

        // Act & Assert
        tier.Matches(500).Should().BeFalse();
        tier.Matches(9999).Should().BeFalse();
    }

    [Fact]
    public void Matches_NullMax_ShouldAlwaysReturnTrue_ForQuantitiesAboveMin()
    {
        // Arrange
        var tier = new PriceTier(ValidId, ValidVariantId, 500, null, 0.0700m);

        // Act & Assert
        tier.Matches(500).Should().BeTrue();
        tier.Matches(1_000).Should().BeTrue();
        tier.Matches(1_000_000).Should().BeTrue();
    }

    [Theory]
    [InlineData(100, 499, 99,   false)]
    [InlineData(100, 499, 100,  true)]
    [InlineData(100, 499, 300,  true)]
    [InlineData(100, 499, 499,  true)]
    [InlineData(100, 499, 500,  false)]
    public void Matches_Boundary_ShouldRespectInclusiveBounds(
        int min, int max, int quantity, bool expected)
    {
        // Arrange
        var tier = new PriceTier(ValidId, ValidVariantId, min, max, 0.10m);

        // Act & Assert
        tier.Matches(quantity).Should().Be(expected);
    }
}
