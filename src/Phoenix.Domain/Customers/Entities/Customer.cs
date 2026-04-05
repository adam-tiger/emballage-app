using Phoenix.Domain.Customers.Events;
using Phoenix.Domain.Customers.Exceptions;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Domain.Customers.Entities;

/// <summary>
/// Agrégat racine représentant un client professionnel de Phoenix Emballages.
/// Encapsule l'identité métier du client, ses adresses et ses événements de domaine.
/// </summary>
/// <remarks>
/// Un <c>Customer</c> est distinct de l'<c>ApplicationUser</c> ASP.NET Identity :
/// l'utilisateur applicatif (Infrastructure) porte les credentials ; le Customer
/// porte les données métier (segment, adresses, commandes).
/// </remarks>
public sealed class Customer
{
    // ── Constantes ───────────────────────────────────────────────────────────

    private const int MaxAddresses      = 5;
    private const int FirstNameMaxLen   = 100;
    private const int LastNameMaxLen    = 100;
    private const int CompanyNameMaxLen = 200;

    // Email simple RFC 5322 lite — suffisant pour une validation de domaine
    private static readonly System.Text.RegularExpressions.Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase |
            System.Text.RegularExpressions.RegexOptions.Compiled);

    // ── Backing fields ───────────────────────────────────────────────────────

    private readonly List<CustomerAddress> _addresses    = [];
    private readonly List<object>          _domainEvents = [];

    // ── Propriétés publiques ─────────────────────────────────────────────────

    /// <summary>Identifiant unique du client dans le domaine Phoenix.</summary>
    public Guid Id { get; private init; }

    /// <summary>
    /// Identifiant de l'utilisateur applicatif ASP.NET Identity associé à ce client.
    /// Clé étrangère vers <c>ApplicationUser.Id</c> dans la couche Infrastructure.
    /// </summary>
    public Guid ApplicationUserId { get; private init; }

    /// <summary>Prénom du client. Maximum <c>100</c> caractères.</summary>
    public string FirstName { get; private set; } = default!;

    /// <summary>Nom de famille du client. Maximum <c>100</c> caractères.</summary>
    public string LastName { get; private set; } = default!;

    /// <summary>Adresse e-mail professionnelle du client (unique, non modifiable après création).</summary>
    public string Email { get; private init; } = default!;

    /// <summary>Raison sociale ou nom commercial. Optionnel. Maximum <c>200</c> caractères.</summary>
    public string? CompanyName { get; private set; }

    /// <summary>Segment d'activité professionnelle du client (ex : FastFood, BakeryPastry).</summary>
    public CustomerSegment Segment { get; private set; }

    /// <summary>Indique si le compte client est actif. Mis à <c>false</c> lors d'une désactivation.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Date de création du profil client (UTC).</summary>
    public DateTime CreatedAtUtc { get; private init; }

    /// <summary>Date de la dernière mise à jour du profil (UTC).</summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Nom complet du client (prénom + nom).
    /// Propriété calculée — non persistée en base.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    // ── Collections ──────────────────────────────────────────────────────────

    /// <summary>Adresses de livraison du client (maximum <see cref="MaxAddresses"/>).</summary>
    public IReadOnlyList<CustomerAddress> Addresses => _addresses.AsReadOnly();

    /// <summary>
    /// Événements de domaine en attente de publication.
    /// Vidés après dispatch par le <c>IUnitOfWork</c>.
    /// </summary>
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    // ── Constructeurs ────────────────────────────────────────────────────────

    /// <summary>Constructeur privé — utiliser <see cref="Create"/> pour instancier.</summary>
    private Customer() { }

    // ── Factory method ───────────────────────────────────────────────────────

    /// <summary>
    /// Crée un nouveau client après validation des invariants de domaine.
    /// Émet un <see cref="CustomerRegisteredEvent"/>.
    /// </summary>
    /// <param name="applicationUserId">Identifiant de l'<c>ApplicationUser</c> associé.</param>
    /// <param name="firstName">Prénom du client (obligatoire, max 100 caractères).</param>
    /// <param name="lastName">Nom de famille (obligatoire, max 100 caractères).</param>
    /// <param name="email">Adresse e-mail valide (obligatoire, unique).</param>
    /// <param name="companyName">Raison sociale optionnelle (max 200 caractères).</param>
    /// <param name="segment">Segment professionnel du client.</param>
    /// <returns>Une nouvelle instance de <see cref="Customer"/>.</returns>
    /// <exception cref="CustomerDomainException">
    /// Levée si un champ obligatoire est manquant ou si l'e-mail est invalide.
    /// </exception>
    public static Customer Create(
        Guid applicationUserId,
        string firstName,
        string lastName,
        string email,
        string? companyName,
        CustomerSegment segment)
    {
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidateEmail(email);

        var now = DateTime.UtcNow;

        var customer = new Customer
        {
            Id                = Guid.NewGuid(),
            ApplicationUserId = applicationUserId,
            FirstName         = firstName.Trim(),
            LastName          = lastName.Trim(),
            Email             = email.Trim().ToLowerInvariant(),
            CompanyName       = string.IsNullOrWhiteSpace(companyName) ? null : companyName.Trim(),
            Segment           = segment,
            IsActive          = true,
            CreatedAtUtc      = now,
            UpdatedAtUtc      = now
        };

        customer._domainEvents.Add(new CustomerRegisteredEvent(
            customer.Id,
            customer.Email,
            customer.FullName,
            now));

        return customer;
    }

    // ── Méthodes publiques ───────────────────────────────────────────────────

    /// <summary>
    /// Met à jour le profil du client et émet un <see cref="CustomerProfileUpdatedEvent"/>.
    /// </summary>
    /// <param name="firstName">Nouveau prénom (obligatoire, max 100 caractères).</param>
    /// <param name="lastName">Nouveau nom de famille (obligatoire, max 100 caractères).</param>
    /// <param name="companyName">Nouvelle raison sociale (optionnelle).</param>
    /// <param name="segment">Nouveau segment professionnel.</param>
    /// <exception cref="CustomerDomainException">Levée si les champs obligatoires sont invalides.</exception>
    public void UpdateProfile(
        string firstName,
        string lastName,
        string? companyName,
        CustomerSegment segment)
    {
        ValidateFirstName(firstName);
        ValidateLastName(lastName);

        FirstName   = firstName.Trim();
        LastName    = lastName.Trim();
        CompanyName = string.IsNullOrWhiteSpace(companyName) ? null : companyName.Trim();
        Segment     = segment;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new CustomerProfileUpdatedEvent(Id, UpdatedAtUtc));
    }

    /// <summary>
    /// Ajoute une adresse de livraison au client.
    /// La première adresse ajoutée devient automatiquement l'adresse par défaut.
    /// Émet un <see cref="CustomerAddressAddedEvent"/>.
    /// </summary>
    /// <param name="address">Adresse à ajouter (déjà construite et validée).</param>
    /// <exception cref="CustomerDomainException">
    /// Levée si le client a déjà atteint le maximum de <see cref="MaxAddresses"/> adresses.
    /// </exception>
    public void AddAddress(CustomerAddress address)
    {
        if (_addresses.Count >= MaxAddresses)
            throw new CustomerDomainException(
                CustomerDomainException.MaxAddressesReached,
                $"Un client ne peut pas avoir plus de {MaxAddresses} adresses de livraison.");

        // Première adresse → défaut automatique
        if (_addresses.Count == 0)
            address.SetAsDefault();

        _addresses.Add(address);
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new CustomerAddressAddedEvent(Id, address.Id, UpdatedAtUtc));
    }

    /// <summary>
    /// Définit une adresse comme adresse par défaut et retire ce statut des autres.
    /// </summary>
    /// <param name="addressId">Identifiant de l'adresse à promouvoir par défaut.</param>
    /// <exception cref="CustomerDomainException">Levée si aucune adresse ne correspond à <paramref name="addressId"/>.</exception>
    public void SetDefaultAddress(Guid addressId)
    {
        var target = _addresses.SingleOrDefault(a => a.Id == addressId)
            ?? throw new CustomerDomainException(
                CustomerDomainException.AddressNotFound,
                $"L'adresse avec l'identifiant '{addressId}' est introuvable.");

        foreach (var addr in _addresses)
            addr.UnsetDefault();

        target.SetAsDefault();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Désactive le compte client. Un compte désactivé ne peut plus passer de commandes.
    /// </summary>
    public void Deactivate()
    {
        IsActive     = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Vide la liste des événements de domaine en attente.
    /// Appelé par <c>IUnitOfWork</c> après le dispatch des événements.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    // ── Validation privée ────────────────────────────────────────────────────

    private static void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new CustomerDomainException(
                CustomerDomainException.FirstNameRequired,
                "Le prénom est obligatoire.");

        if (firstName.Length > FirstNameMaxLen)
            throw new CustomerDomainException(
                CustomerDomainException.FirstNameRequired,
                $"Le prénom ne peut pas dépasser {FirstNameMaxLen} caractères.");
    }

    private static void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new CustomerDomainException(
                CustomerDomainException.LastNameRequired,
                "Le nom de famille est obligatoire.");

        if (lastName.Length > LastNameMaxLen)
            throw new CustomerDomainException(
                CustomerDomainException.LastNameRequired,
                $"Le nom de famille ne peut pas dépasser {LastNameMaxLen} caractères.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new CustomerDomainException(
                CustomerDomainException.EmailRequired,
                "L'adresse e-mail est obligatoire.");

        if (!EmailRegex.IsMatch(email))
            throw new CustomerDomainException(
                CustomerDomainException.InvalidEmail,
                "L'adresse e-mail fournie n'est pas valide.");
    }
}
