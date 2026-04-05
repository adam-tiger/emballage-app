namespace Phoenix.Domain.Customers.Exceptions;

/// <summary>
/// Exception levée lorsqu'un invariant du domaine <c>Customer</c> est violé.
/// Chaque cas métier est identifié par un code string unique (<see cref="Code"/>).
/// </summary>
public sealed class CustomerDomainException : Exception
{
    /// <summary>Code métier identifiant la règle violée (ex : "FIRST_NAME_REQUIRED").</summary>
    public string Code { get; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CustomerDomainException"/>.
    /// </summary>
    /// <param name="code">Code unique identifiant la règle de domaine violée.</param>
    /// <param name="message">Message lisible décrivant la violation.</param>
    public CustomerDomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    // ── Codes d'erreur — Champs Customer ────────────────────────────────────

    /// <summary>Le prénom est obligatoire et ne peut pas être vide.</summary>
    public const string FirstNameRequired = "FIRST_NAME_REQUIRED";

    /// <summary>Le nom de famille est obligatoire et ne peut pas être vide.</summary>
    public const string LastNameRequired = "LAST_NAME_REQUIRED";

    /// <summary>L'adresse e-mail est obligatoire et ne peut pas être vide.</summary>
    public const string EmailRequired = "EMAIL_REQUIRED";

    /// <summary>L'adresse e-mail fournie n'est pas dans un format valide.</summary>
    public const string InvalidEmail = "INVALID_EMAIL";

    // ── Codes d'erreur — Adresses ────────────────────────────────────────────

    /// <summary>Un client ne peut pas avoir plus de 5 adresses de livraison.</summary>
    public const string MaxAddressesReached = "MAX_ADDRESSES_REACHED";

    /// <summary>Aucune adresse ne correspond à l'identifiant fourni.</summary>
    public const string AddressNotFound = "ADDRESS_NOT_FOUND";

    /// <summary>Le code postal fourni est invalide ou trop long.</summary>
    public const string InvalidPostalCode = "INVALID_POSTAL_CODE";

    // ── Codes d'erreur — Champs adresse ─────────────────────────────────────

    /// <summary>La rue / ligne d'adresse est obligatoire et ne peut pas être vide.</summary>
    public const string StreetRequired = "STREET_REQUIRED";

    /// <summary>La ville est obligatoire et ne peut pas être vide.</summary>
    public const string CityRequired = "CITY_REQUIRED";

    /// <summary>Le libellé de l'adresse est obligatoire et ne peut pas être vide.</summary>
    public const string LabelRequired = "LABEL_REQUIRED";
}
