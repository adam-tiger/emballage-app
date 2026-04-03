using Phoenix.Domain.Products.Events;
using Phoenix.Domain.Products.Exceptions;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Domain.Products.Entities;

/// <summary>
/// Agrégat racine du module Product &amp; Catalog.
/// Représente un produit d'emballage avec ses variantes d'impression, ses images et ses 8 flags métier.
/// </summary>
/// <remarks>
/// <b>8 flags booléens :</b>
/// <list type="table">
///   <item><see cref="IsCustomizable"/> — Accepte une impression client personnalisée.</item>
///   <item><see cref="IsGourmetRange"/> — Appartient à la gamme gastronomique premium.</item>
///   <item><see cref="IsBulkOnly"/> — Vendu uniquement en grandes quantités (vrac industriel).</item>
///   <item><see cref="IsEcoFriendly"/> — Éco-responsable / certifié recyclable ou compostable.</item>
///   <item><see cref="IsFoodApproved"/> — Certifié contact alimentaire (norme CE).</item>
///   <item><see cref="SoldByWeight"/> — Vendu au poids plutôt qu'à l'unité.</item>
///   <item><see cref="HasExpressDelivery"/> — Disponible en livraison express J+1.</item>
///   <item><see cref="IsActive"/> — Produit visible et commandable dans le catalogue.</item>
/// </list>
/// <b>Invariants :</b>
/// <list type="bullet">
///   <item><see cref="Sku"/> est non vide et stocké en majuscules.</item>
///   <item>Un seul <see cref="ProductImage"/> peut avoir <c>IsMain = true</c>.</item>
///   <item>Chaque variante a un <c>Sku</c> unique au sein du produit.</item>
/// </list>
/// </remarks>
public sealed class Product
{
    private readonly List<ProductVariant> _variants = [];
    private readonly List<ProductImage> _images = [];
    private readonly List<object> _domainEvents = [];

    /// <summary>
    /// Constructeur privé — utiliser <see cref="Create"/> pour instancier.
    /// Réservé également à EF Core via la reflection.
    /// </summary>
    private Product() { }

    private Product(
        Guid id,
        string sku,
        string nameFr,
        ProductFamily family,
        bool isCustomizable,
        bool isGourmetRange,
        bool isBulkOnly,
        bool isEcoFriendly,
        bool isFoodApproved,
        bool soldByWeight,
        bool hasExpressDelivery)
    {
        Id = id;
        Sku = sku;
        NameFr = nameFr;
        DescriptionFr = string.Empty;
        Family = family;
        IsCustomizable = isCustomizable;
        IsGourmetRange = isGourmetRange;
        IsBulkOnly = isBulkOnly;
        IsEcoFriendly = isEcoFriendly;
        IsFoodApproved = isFoodApproved;
        SoldByWeight = soldByWeight;
        HasExpressDelivery = hasExpressDelivery;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // ── Propriétés ──────────────────────────────────────────────────────────

    /// <summary>Identifiant unique du produit (UUIDv7 — tri chronologique).</summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Référence SKU immuable du produit (ex : "SAC-KRAFT-TORSADE-M").
    /// Unique dans tout le catalogue. Stocké en majuscules.
    /// </summary>
    public string Sku { get; private set; } = string.Empty;

    /// <summary>Libellé commercial en français.</summary>
    public string NameFr { get; private set; } = string.Empty;

    /// <summary>Description longue en français (marketing, caractéristiques techniques).</summary>
    public string DescriptionFr { get; private set; } = string.Empty;

    /// <summary>
    /// Famille de produits du catalogue Phoenix.
    /// </summary>
    /// <remarks>PostgreSQL : stocké en string via HasConversion&lt;string&gt;().</remarks>
    public ProductFamily Family { get; private set; }

    // ── 8 flags booléens ────────────────────────────────────────────────────

    /// <summary>
    /// Le produit accepte une personnalisation par impression client (logo, texte, couleur).
    /// Implique qu'au moins une <see cref="ProductVariant"/> avec options d'impression est configurée.
    /// </summary>
    public bool IsCustomizable { get; private set; }

    /// <summary>
    /// Appartient à la gamme gastronomique/traiteur premium de Phoenix.
    /// Déclenche un affichage spécifique dans le catalogue.
    /// </summary>
    public bool IsGourmetRange { get; private set; }

    /// <summary>
    /// Vendu uniquement en grande quantité (conditionnement vrac industriel).
    /// Implique un <see cref="ProductVariant.MinimumOrderQuantity"/> élevé.
    /// </summary>
    public bool IsBulkOnly { get; private set; }

    /// <summary>
    /// Certifié éco-responsable (recyclable, compostable, biosourcé, FSC, PEFC…).
    /// Affiché avec le label vert dans le catalogue.
    /// </summary>
    public bool IsEcoFriendly { get; private set; }

    /// <summary>
    /// Certifié contact alimentaire selon la réglementation CE 1935/2004.
    /// Obligatoire pour les produits en contact direct avec des aliments.
    /// </summary>
    public bool IsFoodApproved { get; private set; }

    /// <summary>
    /// Vendu au poids (kg) plutôt qu'à l'unité.
    /// Impacte le calcul de prix et la gestion du stock.
    /// </summary>
    public bool SoldByWeight { get; private set; }

    /// <summary>
    /// Disponible en livraison express J+1 depuis le stock Phoenix.
    /// </summary>
    public bool HasExpressDelivery { get; private set; }

    /// <summary>
    /// Produit visible et commandable dans le catalogue public.
    /// <c>false</c> = archivé / retiré de la vente (soft delete).
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>Date et heure de création UTC.</summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>Date et heure de dernière modification UTC.</summary>
    public DateTime UpdatedAtUtc { get; private set; }

    // ── Collections ─────────────────────────────────────────────────────────

    /// <summary>Variantes d'impression disponibles pour ce produit.</summary>
    public IReadOnlyList<ProductVariant> Variants => _variants.AsReadOnly();

    /// <summary>Images du produit (galerie + vignette principale).</summary>
    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();

    /// <summary>
    /// Événements de domaine accumulés depuis la dernière sauvegarde.
    /// Dispatchés par l'Infrastructure après <c>SaveChangesAsync</c>.
    /// </summary>
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    // ── Factory method ──────────────────────────────────────────────────────

    /// <summary>
    /// Crée un nouveau produit, valide les invariants et publie un <see cref="ProductCreatedEvent"/>.
    /// </summary>
    /// <param name="sku">Référence SKU unique et non vide.</param>
    /// <param name="nameFr">Libellé commercial en français (non vide).</param>
    /// <param name="family">Famille de produits du catalogue Phoenix.</param>
    /// <param name="isCustomizable">Personnalisation par impression activée.</param>
    /// <param name="isGourmetRange">Appartient à la gamme gastronomique.</param>
    /// <param name="isBulkOnly">Vendu uniquement en vrac.</param>
    /// <param name="isEcoFriendly">Certifié éco-responsable.</param>
    /// <param name="isFoodApproved">Certifié contact alimentaire.</param>
    /// <param name="soldByWeight">Vendu au poids.</param>
    /// <param name="hasExpressDelivery">Livraison express disponible.</param>
    /// <returns>Nouvel agrégat <see cref="Product"/> avec l'événement <see cref="ProductCreatedEvent"/> en file.</returns>
    /// <exception cref="ProductDomainException">
    /// Code <see cref="ProductDomainException.SkuRequired"/> si <paramref name="sku"/> est vide.
    /// </exception>
    public static Product Create(
        string sku,
        string nameFr,
        ProductFamily family,
        bool isCustomizable,
        bool isGourmetRange,
        bool isBulkOnly,
        bool isEcoFriendly,
        bool isFoodApproved,
        bool soldByWeight,
        bool hasExpressDelivery)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ProductDomainException(
                ProductDomainException.SkuRequired,
                "Le SKU du produit ne peut pas être vide.");

        var product = new Product(
            id: Guid.CreateVersion7(),
            sku: sku.Trim().ToUpperInvariant(),
            nameFr: nameFr?.Trim() ?? string.Empty,
            family: family,
            isCustomizable: isCustomizable,
            isGourmetRange: isGourmetRange,
            isBulkOnly: isBulkOnly,
            isEcoFriendly: isEcoFriendly,
            isFoodApproved: isFoodApproved,
            soldByWeight: soldByWeight,
            hasExpressDelivery: hasExpressDelivery);

        product._domainEvents.Add(new ProductCreatedEvent(product.Id, DateTime.UtcNow));

        return product;
    }

    // ── Commandes métier ────────────────────────────────────────────────────

    /// <summary>
    /// Met à jour le libellé et la description du produit.
    /// Publie un <see cref="ProductUpdatedEvent"/> et met à jour <see cref="UpdatedAtUtc"/>.
    /// </summary>
    /// <param name="nameFr">Nouveau libellé (peut être vide pour conserver l'existant).</param>
    /// <param name="descriptionFr">Nouvelle description longue.</param>
    public void Update(string nameFr, string descriptionFr)
    {
        NameFr = nameFr?.Trim() ?? NameFr;
        DescriptionFr = descriptionFr?.Trim() ?? string.Empty;
        UpdatedAtUtc = DateTime.UtcNow;
        _domainEvents.Add(new ProductUpdatedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Désactive le produit (soft delete — retire du catalogue public).
    /// Publie un <see cref="ProductDeactivatedEvent"/> et met à jour <see cref="UpdatedAtUtc"/>.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
        _domainEvents.Add(new ProductDeactivatedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Ajoute une variante d'impression au produit.
    /// Valide l'unicité du SKU parmi les variantes existantes.
    /// </summary>
    /// <param name="variant">Variante à ajouter.</param>
    /// <exception cref="ProductDomainException">
    /// Code <see cref="ProductDomainException.SkuAlreadyExists"/> si le SKU est déjà utilisé.
    /// </exception>
    public void AddVariant(ProductVariant variant)
    {
        if (_variants.Any(v => v.Sku.Equals(variant.Sku, StringComparison.OrdinalIgnoreCase)))
            throw new ProductDomainException(
                ProductDomainException.SkuAlreadyExists,
                $"Une variante avec le SKU '{variant.Sku}' existe déjà sur le produit '{Sku}'.");

        _variants.Add(variant);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Définit une image comme image principale du produit.
    /// Retire le statut principal de toutes les autres images.
    /// </summary>
    /// <param name="imageId">Identifiant de l'image à promouvoir comme principale.</param>
    /// <exception cref="ProductDomainException">
    /// Code <see cref="ProductDomainException.InvalidImagePath"/> si l'image n'existe pas.
    /// </exception>
    public void SetMainImage(Guid imageId)
    {
        var target = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new ProductDomainException(
                ProductDomainException.InvalidImagePath,
                $"L'image '{imageId}' n'existe pas sur le produit '{Sku}'.");

        foreach (var img in _images)
            img.UnsetAsMain();

        target.SetAsMain();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Vide la liste des événements de domaine après que l'Infrastructure les a dispatchés.
    /// Appelé dans <c>SaveChangesAsync</c> via l'intercepteur EF Core.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
