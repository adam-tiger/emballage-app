using FluentAssertions;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.Exceptions;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Products.Domain;

/// <summary>
/// Tests unitaires de <see cref="ProductVariant"/> :
/// calcul de prix, coefficients d'impression, gestion des paliers.
/// </summary>
public sealed class ProductVariantTests
{
    // ── Helpers ─────────────────────────────────────────────────────────────

    private static ProductVariant CreateVariant(
        PrintSide printSide = PrintSide.SingleSide,
        ColorCount colorCount = ColorCount.One,
        int moq = 250)
    {
        var variant = new ProductVariant(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "SAC-01-1C-R",
            "Sac 1 couleur recto",
            moq,
            printSide,
            colorCount,
            DateTime.UtcNow);

        variant.AddPriceTier(new PriceTier(
            Guid.CreateVersion7(), variant.Id,
            minQuantity: 250, maxQuantity: 499,
            unitPriceHT: 0.0872m));

        variant.AddPriceTier(new PriceTier(
            Guid.CreateVersion7(), variant.Id,
            minQuantity: 500, maxQuantity: null,
            unitPriceHT: 0.0700m));

        return variant;
    }

    // ── GetPriceForQuantity ──────────────────────────────────────────────────

    [Fact]
    public void GetPriceForQuantity_WithValidQuantity_ShouldReturnCorrectTier()
    {
        // Arrange
        var variant = CreateVariant(PrintSide.SingleSide, ColorCount.One);
        // SingleSide coeff=1.00 × One coeff=1.00 → total=1.00

        // Act
        var price300 = variant.GetPriceForQuantity(300);
        var price600 = variant.GetPriceForQuantity(600);

        // Assert
        // 0.0872 × 1.00 = 0.0872
        price300.Amount.Should().Be(Math.Round(0.0872m * 1.00m, 4));
        // 0.0700 × 1.00 = 0.0700
        price600.Amount.Should().Be(Math.Round(0.0700m * 1.00m, 4));
    }

    [Fact]
    public void GetPriceForQuantity_WithPrintCoefficient_ShouldApplyCoefficient()
    {
        // Arrange
        // DoubleSide=1.15 × Two=1.10 → 1.265
        var variant = CreateVariant(PrintSide.DoubleSide, ColorCount.Two);

        // Act
        var price = variant.GetPriceForQuantity(300);

        // Assert
        var expected = Math.Round(0.0872m * 1.15m * 1.10m, 4);
        price.Amount.Should().Be(expected);
    }

    [Fact]
    public void GetPriceForQuantity_BelowMoq_ShouldThrowProductDomainException()
    {
        // Arrange
        var variant = CreateVariant(moq: 250);

        // Act
        Action act = () => variant.GetPriceForQuantity(100);

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.QuantityBelowMinimum);
    }

    [Fact]
    public void GetPriceForQuantity_ExactlyAtMoq_ShouldReturnFirstTier()
    {
        // Arrange
        var variant = CreateVariant(moq: 250);

        // Act
        var price = variant.GetPriceForQuantity(250);

        // Assert
        price.Should().NotBeNull();
        price.Amount.Should().Be(Math.Round(0.0872m, 4));
    }

    // ── CalculatePrintCoefficient ────────────────────────────────────────────

    [Fact]
    public void CalculatePrintCoefficient_SingleSideOne_ShouldReturn1()
    {
        // Arrange
        var variant = CreateVariant(PrintSide.SingleSide, ColorCount.One);

        // Act
        var coeff = variant.CalculatePrintCoefficient();

        // Assert
        coeff.Should().Be(1.00m);
    }

    [Fact]
    public void CalculatePrintCoefficient_DoubleSideFourCmyk_ShouldReturn1Point4375()
    {
        // Arrange
        var variant = CreateVariant(PrintSide.DoubleSide, ColorCount.FourCMYK);

        // Act
        var coeff = variant.CalculatePrintCoefficient();

        // Assert
        coeff.Should().Be(1.15m * 1.25m); // = 1.4375m
    }

    [Fact]
    public void CalculatePrintCoefficient_SingleSideFourCmyk_ShouldReturn1Point25()
    {
        // Arrange
        var variant = CreateVariant(PrintSide.SingleSide, ColorCount.FourCMYK);

        // Act
        var coeff = variant.CalculatePrintCoefficient();

        // Assert
        coeff.Should().Be(1.00m * 1.25m); // = 1.25m
    }

    [Theory]
    [InlineData(PrintSide.SingleSide, ColorCount.One,      1.00)]
    [InlineData(PrintSide.SingleSide, ColorCount.Two,      1.10)]
    [InlineData(PrintSide.SingleSide, ColorCount.Three,    1.18)]
    [InlineData(PrintSide.SingleSide, ColorCount.FourCMYK, 1.25)]
    [InlineData(PrintSide.DoubleSide, ColorCount.One,      1.15)]
    [InlineData(PrintSide.DoubleSide, ColorCount.Two,      1.265)]
    [InlineData(PrintSide.DoubleSide, ColorCount.Three,    1.357)]
    [InlineData(PrintSide.DoubleSide, ColorCount.FourCMYK, 1.4375)]
    public void CalculatePrintCoefficient_AllCombinations_ShouldBeCorrect(
        PrintSide printSide, ColorCount colorCount, double expectedCoeff)
    {
        // Arrange
        var variant = new ProductVariant(
            Guid.CreateVersion7(), Guid.CreateVersion7(),
            "SKU-TEST", "Test", 1, printSide, colorCount, DateTime.UtcNow);

        // Act
        var coeff = variant.CalculatePrintCoefficient();

        // Assert
        coeff.Should().BeApproximately((decimal)expectedCoeff, 0.0001m);
    }

    // ── AddPriceTier ─────────────────────────────────────────────────────────

    [Fact]
    public void AddPriceTier_WithOverlappingRange_ShouldThrowProductDomainException()
    {
        // Arrange
        var variantId = Guid.CreateVersion7();
        var variant = new ProductVariant(
            Guid.CreateVersion7(), Guid.CreateVersion7(),
            "SKU-OVERLAP", "Overlap test", 1,
            PrintSide.SingleSide, ColorCount.One, DateTime.UtcNow);

        variant.AddPriceTier(new PriceTier(
            Guid.CreateVersion7(), variantId,
            minQuantity: 100, maxQuantity: 500, unitPriceHT: 0.10m));

        Action act = () => variant.AddPriceTier(new PriceTier(
            Guid.CreateVersion7(), variantId,
            minQuantity: 300, maxQuantity: 600, unitPriceHT: 0.08m));

        // Assert
        act.Should().Throw<ProductDomainException>()
           .Which.Code.Should().Be(ProductDomainException.InvalidPriceTier);
    }
}
