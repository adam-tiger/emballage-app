namespace Phoenix.Domain.Products.ValueObjects;

// PostgreSQL : stocké en string via HasConversion<string>()

/// <summary>
/// Segment client cible d'un produit du catalogue Phoenix (10 segments).
/// Permet de filtrer et personnaliser l'offre selon le type d'établissement.
/// </summary>
public enum CustomerSegment
{
    /// <summary>Restauration rapide et fast-food — Fast-food.</summary>
    FastFood,

    /// <summary>Boulangerie, pâtisserie, viennoiserie — Boulangerie / Pâtisserie.</summary>
    BakeryPastry,

    /// <summary>Restauration japonaise, asiatique, poké — Japonais / Asiatique.</summary>
    JapaneseAsian,

    /// <summary>Salon de thé bubble tea, smoothies — Bubble Tea.</summary>
    BubbleTea,

    /// <summary>Commerce de détail alimentaire, épicerie fine — Commerce de détail.</summary>
    RetailCommerce,

    /// <summary>Food truck et restauration nomade — Food Truck.</summary>
    FoodTruck,

    /// <summary>Traiteur, buffet, service événementiel — Traiteur.</summary>
    Catering,

    /// <summary>Chocolaterie, confiserie, sucrerie artisanale — Chocolaterie / Confiserie.</summary>
    ChocolateConfectionery,

    /// <summary>Pizzeria et livraison de pizzas — Pizzeria.</summary>
    PizzaShop,

    /// <summary>Autre type d'établissement non listé — Autre.</summary>
    Other
}
