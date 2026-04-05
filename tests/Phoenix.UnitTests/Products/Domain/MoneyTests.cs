using FluentAssertions;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Products.Domain;

/// <summary>
/// Tests unitaires du value object <see cref="Money"/> :
/// opérateurs arithmétiques, TVA, validations et cas limites.
/// </summary>
public sealed class MoneyTests
{
    // ── Construction ─────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidAmount_ShouldSetAmount()
    {
        // Act
        var money = new Money(12.50m);

        // Assert
        money.Amount.Should().Be(12.50m);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Create_NegativeAmount_ShouldThrowArgumentOutOfRangeException()
    {
        // Act
        Action act = () => _ = new Money(-0.01m);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithParameterName("Amount");
    }

    [Fact]
    public void Create_ZeroAmount_ShouldBeAllowed()
    {
        // Act
        var money = new Money(0m);

        // Assert
        money.Amount.Should().Be(0m);
    }

    [Fact]
    public void Zero_ShouldReturnZeroAmount()
    {
        // Act
        var zero = Money.Zero();

        // Assert
        zero.Amount.Should().Be(0m);
        zero.Currency.Should().Be("EUR");
    }

    // ── Addition ─────────────────────────────────────────────────────────────

    [Fact]
    public void Add_TwoMoneyAmounts_ShouldSum()
    {
        // Arrange
        var a = new Money(10.00m);
        var b = new Money(5.50m);

        // Act
        var result = a + b;

        // Assert
        result.Amount.Should().Be(15.50m);
        result.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Add_DifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var eur = new Money(10.00m, "EUR");
        var usd = new Money(10.00m, "USD");

        // Act
        Action act = () => _ = eur + usd;

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*EUR*USD*");
    }

    [Fact]
    public void Add_ZeroMoney_ShouldReturnOriginalAmount()
    {
        // Arrange
        var money = new Money(42.00m);
        var zero  = Money.Zero();

        // Act
        var result = money + zero;

        // Assert
        result.Amount.Should().Be(42.00m);
    }

    // ── Multiplication ───────────────────────────────────────────────────────

    [Fact]
    public void Multiply_MoneyByFactor_ShouldMultiply()
    {
        // Arrange
        var money = new Money(0.0872m);

        // Act
        var result = money * 1.15m;

        // Assert
        result.Amount.Should().Be(Math.Round(0.0872m * 1.15m, 4));
    }

    [Fact]
    public void Multiply_ByZero_ShouldReturnZero()
    {
        // Arrange
        var money = new Money(100m);

        // Act
        var result = money * 0m;

        // Assert
        result.Amount.Should().Be(0m);
    }

    [Fact]
    public void Multiply_ByNegativeFactor_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var money = new Money(100m);

        // Act
        Action act = () => _ = money * -1m;

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithParameterName("factor");
    }

    // ── WithTax ──────────────────────────────────────────────────────────────

    [Fact]
    public void WithTax_ShouldApplyTaxRate()
    {
        // Arrange
        var money = new Money(100m);

        // Act
        var result = money.WithTax(0.20m);

        // Assert
        result.Amount.Should().Be(120m);
    }

    [Fact]
    public void WithTax_DefaultRate_ShouldApply20Percent()
    {
        // Arrange
        var money = new Money(50m);

        // Act
        var result = money.WithTax();

        // Assert
        result.Amount.Should().Be(60m);
    }

    [Fact]
    public void WithTax_Zero_ShouldReturnSameAmount()
    {
        // Arrange
        var money = new Money(100m);

        // Act
        var result = money.WithTax(0m);

        // Assert
        result.Amount.Should().Be(100m);
    }

    // ── Immutabilité ─────────────────────────────────────────────────────────

    [Fact]
    public void Money_IsImmutable_OperationsReturnNewInstance()
    {
        // Arrange
        var original = new Money(100m);

        // Act
        var withTax = original.WithTax(0.20m);

        // Assert
        original.Amount.Should().Be(100m);
        withTax.Amount.Should().Be(120m);
    }

    // ── Equality ─────────────────────────────────────────────────────────────

    [Fact]
    public void Money_SameAmountAndCurrency_ShouldBeEqual()
    {
        // Arrange
        var a = new Money(10.00m, "EUR");
        var b = new Money(10.00m, "EUR");

        // Assert
        a.Should().Be(b);
    }
}
