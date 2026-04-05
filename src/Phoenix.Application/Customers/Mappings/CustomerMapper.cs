using Riok.Mapperly.Abstractions;
using Phoenix.Application.Customers.Dtos;
using Phoenix.Domain.Customers.Entities;

namespace Phoenix.Application.Customers.Mappings;

/// <summary>
/// Mapper Mapperly pour les entités et DTOs de la couche Customer.
/// </summary>
[Mapper]
public sealed partial class CustomerMapper
{
    /// <summary>
    /// Mappe un <see cref="Customer"/> vers un <see cref="CustomerProfileDto"/> complet.
    /// </summary>
    /// <remarks>
    /// <c>Segment</c> est mappé en string via <see cref="MapSegment"/>.
    /// <c>SegmentLabel</c> est calculé via <see cref="MapSegmentLabel"/>.
    /// <c>Addresses</c> est mappé via <see cref="ToAddressDtos"/>.
    /// </remarks>
    [MapProperty(nameof(Customer.Segment), nameof(CustomerProfileDto.Segment),
        Use = nameof(MapSegment))]
    [MapProperty(nameof(Customer.Segment), nameof(CustomerProfileDto.SegmentLabel),
        Use = nameof(MapSegmentLabel))]
    [MapProperty(nameof(Customer.Addresses), nameof(CustomerProfileDto.Addresses))]
    public partial CustomerProfileDto ToProfileDto(Customer customer);

    /// <summary>
    /// Mappe une <see cref="CustomerAddress"/> vers un <see cref="CustomerAddressDto"/>.
    /// </summary>
    public partial CustomerAddressDto ToAddressDto(CustomerAddress address);

    /// <summary>
    /// Mappe une liste d'adresses de domaine vers une liste de DTOs.
    /// </summary>
    public IReadOnlyList<CustomerAddressDto> ToAddressDtos(
        IReadOnlyList<CustomerAddress> addresses)
        => addresses.Select(ToAddressDto).ToList().AsReadOnly();

    // ── Méthodes de conversion privées ──────────────────────────────────────

    private static string MapSegment(Phoenix.Domain.Products.ValueObjects.CustomerSegment segment)
        => segment.ToString();

    private static string MapSegmentLabel(
        Phoenix.Domain.Products.ValueObjects.CustomerSegment segment)
        => segment switch
        {
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.FastFood
                => "Fast Food & Burger",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.BakeryPastry
                => "Boulangerie & Pâtisserie",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.JapaneseAsian
                => "Japonais & Asiatique",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.BubbleTea
                => "Bubble Tea",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.RetailCommerce
                => "Commerce & Retail",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.FoodTruck
                => "Food Truck",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.Catering
                => "Traiteur & Événementiel",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.ChocolateConfectionery
                => "Chocolaterie & Confiserie",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.PizzaShop
                => "Pizzéria",
            Phoenix.Domain.Products.ValueObjects.CustomerSegment.Other
                => "Autre activité",
            _ => segment.ToString()
        };
}
