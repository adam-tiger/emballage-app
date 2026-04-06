using FluentAssertions;
using Phoenix.Domain.Customers.Entities;
using Phoenix.Domain.Customers.Events;
using Phoenix.Domain.Customers.Exceptions;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.UnitTests.Customers.Domain;

/// <summary>
/// Tests unitaires de l'agrégat <see cref="Customer"/>.
/// Vérifie les invariants de domaine, les événements émis et la gestion des adresses.
/// </summary>
public sealed class CustomerTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Customer CreateValidCustomer(
        string firstName   = "Alice",
        string lastName    = "Dupont",
        string email       = "alice@restaurant.fr",
        string? company    = null,
        CustomerSegment seg = CustomerSegment.FastFood)
        => Customer.Create(Guid.NewGuid(), firstName, lastName, email, company, seg);

    private static CustomerAddress CreateValidAddress(Guid customerId)
        => new(customerId, "Mon restaurant", "12 rue de la Paix", "Paris", "75001");

    // ── Create — happy path ──────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ReturnsCustomerWithCorrectProperties()
    {
        var appUserId = Guid.NewGuid();

        var customer = Customer.Create(
            appUserId, "Alice", "Dupont",
            "alice@restaurant.fr", "Le Burger Co",
            CustomerSegment.FastFood);

        customer.Id.Should().NotBe(Guid.Empty);
        customer.ApplicationUserId.Should().Be(appUserId);
        customer.FirstName.Should().Be("Alice");
        customer.LastName.Should().Be("Dupont");
        customer.Email.Should().Be("alice@restaurant.fr");
        customer.CompanyName.Should().Be("Le Burger Co");
        customer.Segment.Should().Be(CustomerSegment.FastFood);
        customer.IsActive.Should().BeTrue();
        customer.FullName.Should().Be("Alice Dupont");
        customer.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_EmitsCustomerRegisteredEvent()
    {
        var customer = CreateValidCustomer();

        customer.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CustomerRegisteredEvent>();

        var evt = (CustomerRegisteredEvent)customer.DomainEvents[0];
        evt.CustomerId.Should().Be(customer.Id);
        evt.Email.Should().Be(customer.Email);
        evt.FullName.Should().Be(customer.FullName);
    }

    [Fact]
    public void Create_TrimsWhitespaceFromNames()
    {
        var customer = Customer.Create(Guid.NewGuid(), "  Alice  ", "  Dupont  ",
            "alice@test.fr", null, CustomerSegment.Other);

        customer.FirstName.Should().Be("Alice");
        customer.LastName.Should().Be("Dupont");
    }

    [Fact]
    public void Create_NormalizesEmailToLowercase()
    {
        var customer = Customer.Create(Guid.NewGuid(), "Alice", "Dupont",
            "Alice@Restaurant.FR", null, CustomerSegment.Other);

        customer.Email.Should().Be("alice@restaurant.fr");
    }

    [Fact]
    public void Create_WithWhitespaceCompanyName_SetsCompanyNameToNull()
    {
        var customer = Customer.Create(Guid.NewGuid(), "Alice", "Dupont",
            "alice@test.fr", "   ", CustomerSegment.Other);

        customer.CompanyName.Should().BeNull();
    }

    // ── Create — validation failures ─────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Create_WithEmptyFirstName_ThrowsCustomerDomainException(string firstName)
    {
        var act = () => Customer.Create(Guid.NewGuid(), firstName, "Dupont",
            "alice@test.fr", null, CustomerSegment.Other);

        act.Should().Throw<CustomerDomainException>()
            .WithMessage("*prénom*")
            .And.Code.Should().Be(CustomerDomainException.FirstNameRequired);
    }

    [Fact]
    public void Create_WithFirstNameExceeding100Chars_ThrowsCustomerDomainException()
    {
        var act = () => Customer.Create(Guid.NewGuid(), new string('A', 101), "Dupont",
            "alice@test.fr", null, CustomerSegment.Other);

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.FirstNameRequired);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Create_WithEmptyLastName_ThrowsCustomerDomainException(string lastName)
    {
        var act = () => Customer.Create(Guid.NewGuid(), "Alice", lastName,
            "alice@test.fr", null, CustomerSegment.Other);

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.LastNameRequired);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing@dot")]
    [InlineData("@nodomain.fr")]
    public void Create_WithInvalidEmail_ThrowsCustomerDomainException(string email)
    {
        var act = () => Customer.Create(Guid.NewGuid(), "Alice", "Dupont",
            email, null, CustomerSegment.Other);

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().BeOneOf(
                CustomerDomainException.EmailRequired,
                CustomerDomainException.InvalidEmail);
    }

    // ── UpdateProfile ─────────────────────────────────────────────────────────

    [Fact]
    public void UpdateProfile_WithValidData_UpdatesProperties()
    {
        var customer = CreateValidCustomer();

        customer.UpdateProfile("Bob", "Martin", "Pizza Star", CustomerSegment.PizzaShop);

        customer.FirstName.Should().Be("Bob");
        customer.LastName.Should().Be("Martin");
        customer.CompanyName.Should().Be("Pizza Star");
        customer.Segment.Should().Be(CustomerSegment.PizzaShop);
        customer.UpdatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void UpdateProfile_EmitsCustomerProfileUpdatedEvent()
    {
        var customer = CreateValidCustomer();
        customer.ClearDomainEvents();

        customer.UpdateProfile("Bob", "Martin", null, CustomerSegment.Other);

        customer.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CustomerProfileUpdatedEvent>();
    }

    [Fact]
    public void UpdateProfile_WithInvalidFirstName_ThrowsCustomerDomainException()
    {
        var customer = CreateValidCustomer();

        var act = () => customer.UpdateProfile("", "Martin", null, CustomerSegment.Other);

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.FirstNameRequired);
    }

    // ── AddAddress ────────────────────────────────────────────────────────────

    [Fact]
    public void AddAddress_FirstAddress_SetsItAsDefault()
    {
        var customer = CreateValidCustomer();
        var address  = CreateValidAddress(customer.Id);

        customer.AddAddress(address);

        customer.Addresses.Should().ContainSingle();
        customer.Addresses[0].IsDefault.Should().BeTrue();
    }

    [Fact]
    public void AddAddress_SecondAddress_DoesNotChangeDefault()
    {
        var customer = CreateValidCustomer();
        var first    = CreateValidAddress(customer.Id);
        var second   = new CustomerAddress(customer.Id, "Entrepôt", "5 rue Rivoli", "Lyon", "69001");

        customer.AddAddress(first);
        customer.AddAddress(second);

        customer.Addresses.Should().HaveCount(2);
        customer.Addresses[0].IsDefault.Should().BeTrue();
        customer.Addresses[1].IsDefault.Should().BeFalse();
    }

    [Fact]
    public void AddAddress_EmitsCustomerAddressAddedEvent()
    {
        var customer = CreateValidCustomer();
        customer.ClearDomainEvents();
        var address  = CreateValidAddress(customer.Id);

        customer.AddAddress(address);

        customer.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CustomerAddressAddedEvent>();
    }

    [Fact]
    public void AddAddress_WhenFiveAddressesExist_ThrowsCustomerDomainException()
    {
        var customer = CreateValidCustomer();
        for (var i = 1; i <= 5; i++)
            customer.AddAddress(new CustomerAddress(customer.Id, $"Adresse {i}", $"{i} Rue Test", "Paris", "75001"));

        var extra = CreateValidAddress(customer.Id);
        var act   = () => customer.AddAddress(extra);

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.MaxAddressesReached);
    }

    // ── SetDefaultAddress ─────────────────────────────────────────────────────

    [Fact]
    public void SetDefaultAddress_ValidId_ChangesDefault()
    {
        var customer = CreateValidCustomer();
        var first    = CreateValidAddress(customer.Id);
        var second   = new CustomerAddress(customer.Id, "Entrepôt", "5 rue Rivoli", "Lyon", "69001");
        customer.AddAddress(first);
        customer.AddAddress(second);

        customer.SetDefaultAddress(second.Id);

        customer.Addresses.Single(a => a.Id == second.Id).IsDefault.Should().BeTrue();
        customer.Addresses.Single(a => a.Id == first.Id).IsDefault.Should().BeFalse();
    }

    [Fact]
    public void SetDefaultAddress_UnknownId_ThrowsCustomerDomainException()
    {
        var customer = CreateValidCustomer();

        var act = () => customer.SetDefaultAddress(Guid.NewGuid());

        act.Should().Throw<CustomerDomainException>()
            .And.Code.Should().Be(CustomerDomainException.AddressNotFound);
    }

    // ── Deactivate ────────────────────────────────────────────────────────────

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var customer = CreateValidCustomer();

        customer.Deactivate();

        customer.IsActive.Should().BeFalse();
    }

    // ── ClearDomainEvents ─────────────────────────────────────────────────────

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var customer = CreateValidCustomer();
        customer.DomainEvents.Should().NotBeEmpty();

        customer.ClearDomainEvents();

        customer.DomainEvents.Should().BeEmpty();
    }
}
