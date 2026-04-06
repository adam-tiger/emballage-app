using FluentAssertions;
using Phoenix.Domain.Customers.Entities;
using Phoenix.Domain.Customers.Exceptions;

namespace Phoenix.UnitTests.Customers.Domain;

/// <summary>
/// Tests unitaires de l'entité <see cref="CustomerAddress"/>.
/// Vérifie les invariants de construction et les méthodes SetAsDefault/UnsetDefault.
/// </summary>
public sealed class CustomerAddressTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();

    // ── Constructor — happy path ──────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_CreatesAddress()
    {
        var address = new CustomerAddress(
            CustomerId, "Mon restaurant", "12 rue de la Paix", "Paris", "75001", "FR");

        address.Id.Should().NotBe(Guid.Empty);
        address.CustomerId.Should().Be(CustomerId);
        address.Label.Should().Be("Mon restaurant");
        address.Street.Should().Be("12 rue de la Paix");
        address.City.Should().Be("Paris");
        address.PostalCode.Should().Be("75001");
        address.Country.Should().Be("FR");
        address.IsDefault.Should().BeFalse();
        address.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Constructor_WithoutCountry_DefaultsToFR()
    {
        var address = new CustomerAddress(CustomerId, "Label", "Rue", "City", "12345");

        address.Country.Should().Be("FR");
    }

    [Fact]
    public void Constructor_NormalizesCountryToUppercase()
    {
        var address = new CustomerAddress(CustomerId, "Label", "Rue", "City", "12345", "fr");

        address.Country.Should().Be("FR");
    }

    [Fact]
    public void Constructor_TrimsFields()
    {
        var address = new CustomerAddress(
            CustomerId, "  Label  ", "  Rue  ", "  Paris  ", "  75001  ");

        address.Label.Should().Be("Label");
        address.Street.Should().Be("Rue");
        address.City.Should().Be("Paris");
        address.PostalCode.Should().Be("75001");
    }

    [Fact]
    public void Constructor_WithEmptyCountry_DefaultsToFR()
    {
        var address = new CustomerAddress(CustomerId, "Label", "Rue", "City", "12345", "   ");

        address.Country.Should().Be("FR");
    }

    // ── Label validation ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyLabel_ThrowsCustomerDomainException(string label)
    {
        var act = () => new CustomerAddress(CustomerId, label, "Rue Test", "Paris", "75001");

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.LabelRequired);
    }

    [Fact]
    public void Constructor_WithLabelExceeding100Chars_ThrowsCustomerDomainException()
    {
        var act = () => new CustomerAddress(
            CustomerId, new string('A', 101), "Rue Test", "Paris", "75001");

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.LabelRequired);
    }

    // ── Street validation ─────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyStreet_ThrowsCustomerDomainException(string street)
    {
        var act = () => new CustomerAddress(CustomerId, "Label", street, "Paris", "75001");

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.StreetRequired);
    }

    [Fact]
    public void Constructor_WithStreetExceeding200Chars_ThrowsCustomerDomainException()
    {
        var act = () => new CustomerAddress(
            CustomerId, "Label", new string('A', 201), "Paris", "75001");

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.StreetRequired);
    }

    // ── City validation ───────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyCity_ThrowsCustomerDomainException(string city)
    {
        var act = () => new CustomerAddress(CustomerId, "Label", "Rue Test", city, "75001");

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.CityRequired);
    }

    // ── PostalCode validation ─────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyPostalCode_ThrowsCustomerDomainException(string postalCode)
    {
        var act = () => new CustomerAddress(CustomerId, "Label", "Rue Test", "Paris", postalCode);

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.InvalidPostalCode);
    }

    [Fact]
    public void Constructor_WithPostalCodeExceeding10Chars_ThrowsCustomerDomainException()
    {
        var act = () => new CustomerAddress(
            CustomerId, "Label", "Rue Test", "Paris", "12345678901");

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.InvalidPostalCode);
    }

    // ── SetAsDefault / UnsetDefault ───────────────────────────────────────────

    [Fact]
    public void SetAsDefault_SetsIsDefaultTrue()
    {
        var address = new CustomerAddress(CustomerId, "Label", "Rue", "Paris", "75001");
        address.IsDefault.Should().BeFalse();

        address.SetAsDefault();

        address.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void UnsetDefault_SetsIsDefaultFalse()
    {
        var address = new CustomerAddress(CustomerId, "Label", "Rue", "Paris", "75001");
        address.SetAsDefault();

        address.UnsetDefault();

        address.IsDefault.Should().BeFalse();
    }
}
