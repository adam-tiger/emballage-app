using Phoenix.Domain.Catalog.Entities;
using Phoenix.Domain.Catalog.Events;
using Phoenix.Domain.Catalog.ValueObjects;
using Phoenix.Domain.Common.Primitives;

namespace Phoenix.Domain.Catalog.Aggregates;

/// <summary>
/// Agrégat racine du catalogue produit Phoenix.
/// Représente un produit d'emballage avec toutes ses déclinaisons, paliers tarifaires et images.
/// </summary>
/// <remarks>
/// <b>8 flags booléens :</b>
/// <list type="table">
///   <item><see cref="IsActive"/> — Actif dans le catalogue public.</item>
///   <item><see cref="IsCustomizable"/> — Peut être personnalisé avec impression client.</item>
///   <item><see cref="IsEcoFriendly"/> — Éco-responsable / certifié recyclable ou biosourcé.</item>
///   <item><see cref="IsStockItem"/> — Disponible sur stock immédiat (sinon : délai fabrication).</item>
///   <item><see cref="IsNew"/> — Nouveau produit (marqueur "Nouveauté" sur le catalogue).</item>
///   <item><see cref="IsOnPromotion"/> — En promotion (prix réduit affiché).</item>
///   <item><see cref="RequiresArtwork"/> — Nécessite un fichier BAT/artwork de la part du client.</item>
///   <item><see cref="IsExclusive"/> — Produit exclusif, uniquement disponible sur commande spéciale.</item>
/// </list>
/// <b>Règles d'invariants :</b>
/// <list type="bullet">
///   <item>La référence produit est non vide et immuable après création.</item>
///   <item>Les paliers tarifaires ne doivent pas se chevaucher.</item>
///   <item>Une seule image peut avoir <c>IsMain = true</c>.</item>
///   <item>Un produit ne peut pas être désactivé s'il est déjà inactif.</item>
/// </list>
/// </remarks>
public sealed class Product : Entity
{
    private readonly List<ProductVariant> _variants = [];
    private readonly List<PriceTier> _priceTiers = [];
    private readonly List<ProductImage> _images = [];

    /// <summary>
    /// Constructeur sans paramètre réservé à EF Core.
    /// </summary>
    private Product() { }

    private Product(
        Guid id,
        string reference,
        string name,
        string description,
        ProductFamily family,
        CustomerSegment targetSegment)
        : base(id)
    {
        Reference = reference;
        Name = name;
        Description = description;
        Family = family;
        TargetSegment = targetSegment;

        // Flags par défaut à la création
        IsActive = true;
        IsCustomizable = false;
        IsEcoFriendly = false;
        IsStockItem = false;
        IsNew = true;
        IsOnPromotion = false;
        RequiresArtwork = false;
        IsExclusive = false;
    }

    // =========================================================================
    // Propriétés d'identité
    // =========================================================================

    /// <summary>
    /// Référence SKU immuable du produit (ex : "BOX-KRAFT-A4").
    /// Unique dans tout le catalogue Phoenix.
    /// </summary>
    public string Reference { get; private set; } = string.Empty;

    /// <summary>Libellé commercial du produit.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Description longue du produit (marketing, caractéristiques techniques).</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Famille de produits du catalogue Phoenix.
    /// </summary>
    /// <remarks>Stocké en PostgreSQL en tant que <c>varchar</c> via HasConversion&lt;string&gt;().</remarks>
    public ProductFamily Family { get; private set; }

    /// <summary>
    /// Segment client principal visé par ce produit.
    /// </summary>
    /// <remarks>Stocké en PostgreSQL en tant que <c>varchar</c> via HasConversion&lt;string&gt;().</remarks>
    public CustomerSegment TargetSegment { get; private set; }

    // =========================================================================
    // 8 flags booléens
    // =========================================================================

    /// <summary>
    /// Le produit est visible et commandable dans le catalogue public.
    /// <c>false</c> = archivé / retiré de la vente.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Le produit accepte une personnalisation par impression client (logo, texte…).
    /// Implique qu'au moins une variante avec <see cref="ProductVariant.PrintSide"/> existe.
    /// </summary>
    public bool IsCustomizable { get; private set; }

    /// <summary>
    /// Le produit est certifié éco-responsable (recyclable, biosourcé, FSC, PEFC…).
    /// </summary>
    public bool IsEcoFriendly { get; private set; }

    /// <summary>
    /// Le produit est disponible immédiatement depuis le stock Phoenix.
    /// <c>false</c> = produit fabriqué à la commande avec délai.
    /// </summary>
    public bool IsStockItem { get; private set; }

    /// <summary>
    /// Produit récemment introduit au catalogue (marqueur « Nouveauté »).
    /// À passer à <c>false</c> manuellement après 3 mois via la commande de domaine dédiée.
    /// </summary>
    public bool IsNew { get; private set; }

    /// <summary>
    /// Produit actuellement en promotion commerciale (prix réduit, offre spéciale).
    /// </summary>
    public bool IsOnPromotion { get; private set; }

    /// <summary>
    /// La commande de ce produit nécessite que le client fournisse un fichier artwork (BAT).
    /// Déclenche un workflow de validation graphique dans le module Order.
    /// </summary>
    public bool RequiresArtwork { get; private set; }

    /// <summary>
    /// Produit exclusif disponible uniquement sur commande spéciale (hors catalogue standard).
    /// Nécessite une validation commerciale avant confirmation.
    /// </summary>
    public bool IsExclusive { get; private set; }

    // =========================================================================
    // Collections (lecture seule exposée, modifications via méthodes)
    // =========================================================================

    /// <summary>Variantes d'impression disponibles pour ce produit.</summary>
    public IReadOnlyList<ProductVariant> Variants => _variants.AsReadOnly();

    /// <summary>Paliers tarifaires triés par quantité minimale croissante.</summary>
    public IReadOnlyList<PriceTier> PriceTiers => _priceTiers.AsReadOnly();

    /// <summary>Images du produit (galerie + vignette principale).</summary>
    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();

    // =========================================================================
    // Factory method
    // =========================================================================

    /// <summary>
    /// Crée un nouveau produit et publie un <see cref="ProductCreatedEvent"/>.
    /// </summary>
    /// <param name="reference">Référence SKU unique (non vide).</param>
    /// <param name="name">Libellé commercial (non vide).</param>
    /// <param name="description">Description longue (peut être vide).</param>
    /// <param name="family">Famille de produits.</param>
    /// <param name="targetSegment">Segment client cible.</param>
    public static Product Create(
        string reference,
        string name,
        string description,
        ProductFamily family,
        CustomerSegment targetSegment)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("La référence produit ne peut pas être vide.", nameof(reference));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le libellé du produit ne peut pas être vide.", nameof(name));

        if (!Enum.IsDefined(family))
            throw new ArgumentOutOfRangeException(nameof(family), $"Famille de produit invalide : {family}.");

        if (!Enum.IsDefined(targetSegment))
            throw new ArgumentOutOfRangeException(nameof(targetSegment), $"Segment client invalide : {targetSegment}.");

        var product = new Product(
            id: Guid.CreateVersion7(),
            reference: reference.Trim().ToUpperInvariant(),
            name: name.Trim(),
            description: description?.Trim() ?? string.Empty,
            family: family,
            targetSegment: targetSegment);

        product.RaiseDomainEvent(new ProductCreatedEvent(product.Id, product.Reference, product.Family));

        return product;
    }

    // =========================================================================
    // Commandes métier — mise à jour des propriétés
    // =========================================================================

    /// <summary>
    /// Met à jour les informations descriptives du produit et publie un <see cref="ProductUpdatedEvent"/>.
    /// </summary>
    public void Update(string name, string description, CustomerSegment targetSegment)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le libellé du produit ne peut pas être vide.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        TargetSegment = targetSegment;

        RaiseDomainEvent(new ProductUpdatedEvent(Id, Reference));
    }

    /// <summary>
    /// Désactive le produit (retire du catalogue public) et publie un <see cref="ProductDeactivatedEvent"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si le produit est déjà inactif.</exception>
    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException(
                $"Le produit '{Reference}' est déjà inactif.");

        IsActive = false;
        RaiseDomainEvent(new ProductDeactivatedEvent(Id, Reference));
    }

    /// <summary>Réactive un produit archivé.</summary>
    /// <exception cref="InvalidOperationException">Si le produit est déjà actif.</exception>
    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException(
                $"Le produit '{Reference}' est déjà actif.");

        IsActive = true;
        RaiseDomainEvent(new ProductUpdatedEvent(Id, Reference));
    }

    // =========================================================================
    // Commandes métier — flags
    // =========================================================================

    /// <summary>Active ou désactive la personnalisation par impression.</summary>
    public void SetCustomizable(bool value) => IsCustomizable = value;

    /// <summary>Active ou désactive le label éco-responsable.</summary>
    public void SetEcoFriendly(bool value) => IsEcoFriendly = value;

    /// <summary>Active ou désactive la disponibilité stock immédiat.</summary>
    public void SetStockItem(bool value) => IsStockItem = value;

    /// <summary>Active ou désactive le marqueur « Nouveauté ».</summary>
    public void SetNew(bool value) => IsNew = value;

    /// <summary>Active ou désactive le marqueur « Promotion ».</summary>
    public void SetOnPromotion(bool value) => IsOnPromotion = value;

    /// <summary>Active ou désactive l'exigence d'un fichier artwork client.</summary>
    public void SetRequiresArtwork(bool value) => RequiresArtwork = value;

    /// <summary>Active ou désactive le statut « produit exclusif ».</summary>
    public void SetExclusive(bool value) => IsExclusive = value;

    // =========================================================================
    // Commandes métier — variantes
    // =========================================================================

    /// <summary>
    /// Ajoute une variante d'impression à ce produit.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Si une variante avec la même référence existe déjà sur ce produit.
    /// </exception>
    public ProductVariant AddVariant(
        string reference,
        string name,
        int moq,
        PrintSide printSide,
        ColorCount colorCount)
    {
        if (_variants.Any(v => v.Reference.Equals(reference.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException(
                $"Une variante avec la référence '{reference}' existe déjà sur le produit '{Reference}'.");

        var variant = ProductVariant.Create(Id, reference, name, moq, printSide, colorCount);
        _variants.Add(variant);
        return variant;
    }

    /// <summary>
    /// Supprime une variante par son identifiant.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si la variante n'appartient pas à ce produit.</exception>
    public void RemoveVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new InvalidOperationException(
                $"La variante '{variantId}' n'existe pas sur le produit '{Reference}'.");

        _variants.Remove(variant);
    }

    // =========================================================================
    // Commandes métier — paliers tarifaires
    // =========================================================================

    /// <summary>
    /// Ajoute un palier tarifaire en vérifiant l'absence de chevauchement.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si le palier chevauche un palier existant.</exception>
    public PriceTier AddPriceTier(int minQuantity, int? maxQuantity, Money unitPriceHT)
    {
        // Vérification anti-chevauchement
        foreach (var existing in _priceTiers)
        {
            bool overlaps = minQuantity <= (existing.MaxQuantity ?? int.MaxValue)
                            && (maxQuantity ?? int.MaxValue) >= existing.MinQuantity;

            if (overlaps)
                throw new InvalidOperationException(
                    $"Le palier [{minQuantity}–{maxQuantity?.ToString() ?? "∞"}] chevauche " +
                    $"le palier existant [{existing.MinQuantity}–{existing.MaxQuantity?.ToString() ?? "∞"}] " +
                    $"sur le produit '{Reference}'.");
        }

        var tier = PriceTier.Create(Id, minQuantity, maxQuantity, unitPriceHT);
        _priceTiers.Add(tier);
        return tier;
    }

    /// <summary>
    /// Supprime un palier tarifaire par son identifiant.
    /// </summary>
    public void RemovePriceTier(Guid tierId)
    {
        var tier = _priceTiers.FirstOrDefault(t => t.Id == tierId)
            ?? throw new InvalidOperationException(
                $"Le palier '{tierId}' n'existe pas sur le produit '{Reference}'.");

        _priceTiers.Remove(tier);
    }

    /// <summary>
    /// Retourne le prix unitaire HT applicable pour une quantité donnée,
    /// ou <c>null</c> si aucun palier ne couvre cette quantité.
    /// </summary>
    public Money? GetPriceForQuantity(int quantity)
    {
        var tier = _priceTiers
            .OrderByDescending(t => t.MinQuantity)
            .FirstOrDefault(t => t.Covers(quantity));

        return tier?.UnitPriceHT;
    }

    // =========================================================================
    // Commandes métier — images
    // =========================================================================

    /// <summary>
    /// Ajoute une image au produit.
    /// Si c'est la première image ou si <paramref name="isMain"/> = <c>true</c>,
    /// elle devient l'image principale (l'ancienne perd son statut).
    /// </summary>
    public ProductImage AddImage(string blobPath, string publicUrl, bool isMain = false)
    {
        // La première image devient automatiquement l'image principale
        bool shouldBeMain = isMain || !_images.Any();

        if (shouldBeMain)
        {
            // Retirer le statut principal à l'ancienne image principale
            foreach (var img in _images.Where(i => i.IsMain))
                img.UnsetAsMain();
        }

        int sortOrder = _images.Count;
        var image = ProductImage.Create(Id, blobPath, publicUrl, shouldBeMain, sortOrder);
        _images.Add(image);
        return image;
    }

    /// <summary>
    /// Définit une image existante comme image principale.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si l'image n'appartient pas à ce produit.</exception>
    public void SetMainImage(Guid imageId)
    {
        var newMain = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new InvalidOperationException(
                $"L'image '{imageId}' n'existe pas sur le produit '{Reference}'.");

        foreach (var img in _images.Where(i => i.IsMain && i.Id != imageId))
            img.UnsetAsMain();

        newMain.SetAsMain();
    }

    /// <summary>
    /// Supprime une image par son identifiant.
    /// Si l'image supprimée était principale, la première image restante devient principale.
    /// </summary>
    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new InvalidOperationException(
                $"L'image '{imageId}' n'existe pas sur le produit '{Reference}'.");

        bool wasMain = image.IsMain;
        _images.Remove(image);

        if (wasMain && _images.Count > 0)
            _images[0].SetAsMain();
    }
}
