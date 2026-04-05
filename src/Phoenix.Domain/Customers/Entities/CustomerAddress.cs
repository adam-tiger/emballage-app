using Phoenix.Domain.Customers.Exceptions;

namespace Phoenix.Domain.Customers.Entities;

/// <summary>
/// Entité représentant une adresse de livraison appartenant à un <see cref="Customer"/>.
/// N'est pas un agrégat racine — ne peut être manipulée qu'au travers du Customer.
/// </summary>
public sealed class CustomerAddress
{
    // ── Invariants ───────────────────────────────────────────────────────────

    private const int LabelMaxLength      = 100;
    private const int StreetMaxLength     = 200;
    private const int CityMaxLength       = 100;
    private const int PostalCodeMaxLength = 10;

    // ── Propriétés ───────────────────────────────────────────────────────────

    /// <summary>Identifiant unique de l'adresse.</summary>
    public Guid Id { get; private init; }

    /// <summary>Identifiant du client propriétaire de cette adresse.</summary>
    public Guid CustomerId { get; private init; }

    /// <summary>
    /// Libellé fonctionnel de l'adresse (ex : "Mon restaurant", "Entrepôt principal").
    /// Maximum <c>100</c> caractères.
    /// </summary>
    public string Label { get; private set; } = default!;

    /// <summary>Numéro et nom de la rue. Maximum <c>200</c> caractères.</summary>
    public string Street { get; private set; } = default!;

    /// <summary>Ville. Maximum <c>100</c> caractères.</summary>
    public string City { get; private set; } = default!;

    /// <summary>Code postal. Maximum <c>10</c> caractères.</summary>
    public string PostalCode { get; private set; } = default!;

    /// <summary>Code pays ISO 3166-1 alpha-2. Valeur par défaut : <c>"FR"</c>.</summary>
    public string Country { get; private set; } = "FR";

    /// <summary>
    /// Indique si cette adresse est l'adresse par défaut du client.
    /// Une seule adresse par client peut être marquée <c>true</c>.
    /// </summary>
    public bool IsDefault { get; internal set; } = false;

    /// <summary>Date de création de l'adresse (UTC).</summary>
    public DateTime CreatedAtUtc { get; private init; }

    // ── Constructeur ─────────────────────────────────────────────────────────

    /// <summary>
    /// Initialise et valide une nouvelle adresse de livraison.
    /// </summary>
    /// <param name="customerId">Identifiant du client propriétaire.</param>
    /// <param name="label">Libellé fonctionnel de l'adresse.</param>
    /// <param name="street">Rue et numéro.</param>
    /// <param name="city">Ville.</param>
    /// <param name="postalCode">Code postal.</param>
    /// <param name="country">Code pays (par défaut "FR").</param>
    /// <exception cref="CustomerDomainException">
    /// Levée si l'un des champs obligatoires est vide ou dépasse la longueur maximale autorisée.
    /// </exception>
    public CustomerAddress(
        Guid customerId,
        string label,
        string street,
        string city,
        string postalCode,
        string country = "FR")
    {
        ValidateLabel(label);
        ValidateStreet(street);
        ValidateCity(city);
        ValidatePostalCode(postalCode);

        Id          = Guid.NewGuid();
        CustomerId  = customerId;
        Label       = label.Trim();
        Street      = street.Trim();
        City        = city.Trim();
        PostalCode  = postalCode.Trim();
        Country     = string.IsNullOrWhiteSpace(country) ? "FR" : country.Trim().ToUpperInvariant();
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Constructeur sans paramètre requis par EF Core.</summary>
    private CustomerAddress() { }

    // ── Méthodes ─────────────────────────────────────────────────────────────

    /// <summary>Marque cette adresse comme adresse par défaut du client.</summary>
    public void SetAsDefault() => IsDefault = true;

    /// <summary>Retire le statut d'adresse par défaut de cette adresse.</summary>
    public void UnsetDefault() => IsDefault = false;

    // ── Validation privée ────────────────────────────────────────────────────

    private static void ValidateLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new CustomerDomainException(
                CustomerDomainException.LabelRequired,
                "Le libellé de l'adresse est obligatoire.");

        if (label.Length > LabelMaxLength)
            throw new CustomerDomainException(
                CustomerDomainException.LabelRequired,
                $"Le libellé de l'adresse ne peut pas dépasser {LabelMaxLength} caractères.");
    }

    private static void ValidateStreet(string street)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new CustomerDomainException(
                CustomerDomainException.StreetRequired,
                "La rue est obligatoire.");

        if (street.Length > StreetMaxLength)
            throw new CustomerDomainException(
                CustomerDomainException.StreetRequired,
                $"La rue ne peut pas dépasser {StreetMaxLength} caractères.");
    }

    private static void ValidateCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new CustomerDomainException(
                CustomerDomainException.CityRequired,
                "La ville est obligatoire.");

        if (city.Length > CityMaxLength)
            throw new CustomerDomainException(
                CustomerDomainException.CityRequired,
                $"La ville ne peut pas dépasser {CityMaxLength} caractères.");
    }

    private static void ValidatePostalCode(string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new CustomerDomainException(
                CustomerDomainException.InvalidPostalCode,
                "Le code postal est obligatoire.");

        if (postalCode.Length > PostalCodeMaxLength)
            throw new CustomerDomainException(
                CustomerDomainException.InvalidPostalCode,
                $"Le code postal ne peut pas dépasser {PostalCodeMaxLength} caractères.");
    }
}
