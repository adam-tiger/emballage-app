using Phoenix.Domain.Products.Exceptions;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Domain.Products.Entities;

/// <summary>
/// Variante d'impression d'un produit du catalogue Phoenix.
/// Une variante est caractérisée par son SKU, sa face d'impression, son nombre de couleurs
/// et sa quantité minimale de commande (MOQ).
/// </summary>
/// <remarks>
/// <b>Invariants métier :</b>
/// <list type="bullet">
///   <item><see cref="MinimumOrderQuantity"/> >= 1.</item>
///   <item><see cref="Sku"/> est unique par produit (vérifié dans <see cref="Product.AddVariant"/>).</item>
///   <item>Les paliers tarifaires ne peuvent pas se chevaucher.</item>
/// </list>
/// </remarks>
public sealed class ProductVariant
{
    private readonly List<PriceTier> _priceTiers = [];

    /// <summary>
    /// Constructeur public avec validation des invariants métier.
    /// </summary>
    /// <param name="id">Identifiant unique de la variante.</param>
    /// <param name="productId">Identifiant du produit parent.</param>
    /// <param name="sku">Référence SKU unique de la variante (ex : "SAC-KRAFT-1C-R").</param>
    /// <param name="nameFr">Libellé commercial en français.</param>
    /// <param name="minimumOrderQuantity">Quantité minimale de commande (>= 1).</param>
    /// <param name="printSide">Face(s) imprimée(s).</param>
    /// <param name="colorCount">Nombre de couleurs d'impression.</param>
    /// <param name="createdAtUtc">Date de création UTC.</param>
    public ProductVariant(
        Guid id,
        Guid productId,
        string sku,
        string nameFr,
        int minimumOrderQuantity,
        PrintSide printSide,
        ColorCount colorCount,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("L'identifiant de la variante ne peut pas être vide.", nameof(id));

        if (productId == Guid.Empty)
            throw new ArgumentException("L'identifiant du produit ne peut pas être vide.", nameof(productId));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ProductDomainException(
                ProductDomainException.SkuRequired,
                "Le SKU de la variante ne peut pas être vide.");

        if (string.IsNullOrWhiteSpace(nameFr))
            throw new ArgumentException("Le libellé de la variante ne peut pas être vide.", nameof(nameFr));

        if (minimumOrderQuantity < 1)
            throw new ProductDomainException(
                ProductDomainException.InvalidPriceTier,
                $"Le MOQ doit être >= 1 (valeur : {minimumOrderQuantity}).");

        Id = id;
        ProductId = productId;
        Sku = sku.Trim().ToUpperInvariant();
        NameFr = nameFr.Trim();
        MinimumOrderQuantity = minimumOrderQuantity;
        PrintSide = printSide;
        ColorCount = colorCount;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>Identifiant unique de la variante.</summary>
    public Guid Id { get; private set; }

    /// <summary>Identifiant du produit auquel cette variante appartient.</summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Référence SKU de la variante (ex : "SAC-KRAFT-1C-R").
    /// Stockée en majuscules. Unique parmi toutes les variantes du produit parent.
    /// </summary>
    public string Sku { get; private set; }

    /// <summary>Libellé commercial en français de la variante.</summary>
    public string NameFr { get; private set; }

    /// <summary>
    /// Quantité minimale de commande — Minimum Order Quantity.
    /// Doit être >= 1.
    /// </summary>
    public int MinimumOrderQuantity { get; private set; }

    /// <summary>
    /// Face(s) imprimée(s) : recto uniquement (<see cref="PrintSide.SingleSide"/>) ou
    /// recto-verso (<see cref="PrintSide.DoubleSide"/>).
    /// </summary>
    /// <remarks>PostgreSQL : stocké en string via HasConversion&lt;string&gt;().</remarks>
    public PrintSide PrintSide { get; private set; }

    /// <summary>
    /// Nombre de couleurs d'impression (1, 2, 3 ou 4 CMJN).
    /// </summary>
    /// <remarks>PostgreSQL : stocké en string via HasConversion&lt;string&gt;().</remarks>
    public ColorCount ColorCount { get; private set; }

    /// <summary>Date et heure de création UTC.</summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>Paliers tarifaires en lecture seule.</summary>
    public IReadOnlyList<PriceTier> PriceTiers => _priceTiers.AsReadOnly();

    // ── Méthodes métier ─────────────────────────────────────────────────────

    /// <summary>
    /// Ajoute un palier tarifaire en vérifiant l'absence de chevauchement avec les paliers existants.
    /// </summary>
    /// <param name="tier">Palier tarifaire à ajouter.</param>
    /// <exception cref="ProductDomainException">
    /// Code <see cref="ProductDomainException.InvalidPriceTier"/> si chevauchement détecté.
    /// </exception>
    public void AddPriceTier(PriceTier tier)
    {
        foreach (var existing in _priceTiers)
        {
            bool overlaps = tier.MinQuantity <= (existing.MaxQuantity ?? int.MaxValue)
                         && (tier.MaxQuantity ?? int.MaxValue) >= existing.MinQuantity;

            if (overlaps)
                throw new ProductDomainException(
                    ProductDomainException.InvalidPriceTier,
                    $"Le palier [{tier.MinQuantity}–{tier.MaxQuantity?.ToString() ?? "∞"}] chevauche " +
                    $"le palier existant [{existing.MinQuantity}–{existing.MaxQuantity?.ToString() ?? "∞"}].");
        }

        _priceTiers.Add(tier);
    }

    /// <summary>
    /// Calcule le prix unitaire HT pour une quantité donnée en appliquant
    /// le coefficient d'impression de la variante.
    /// </summary>
    /// <param name="quantity">Quantité commandée.</param>
    /// <returns>
    /// <see cref="Money"/> représentant le prix unitaire HT après application du coefficient impression.
    /// </returns>
    /// <exception cref="ProductDomainException">
    /// <see cref="ProductDomainException.QuantityBelowMinimum"/> si quantité &lt; <see cref="MinimumOrderQuantity"/>.
    /// <see cref="ProductDomainException.InvalidPriceTier"/> si aucun palier ne couvre la quantité.
    /// </exception>
    public Money GetPriceForQuantity(int quantity)
    {
        if (quantity < MinimumOrderQuantity)
            throw new ProductDomainException(
                ProductDomainException.QuantityBelowMinimum,
                $"La quantité commandée ({quantity}) est inférieure au MOQ de {MinimumOrderQuantity} " +
                $"pour la variante '{Sku}'.");

        var tier = _priceTiers
            .OrderByDescending(t => t.MinQuantity)
            .FirstOrDefault(t => t.Matches(quantity))
            ?? throw new ProductDomainException(
                ProductDomainException.InvalidPriceTier,
                $"Aucun palier tarifaire ne correspond à la quantité {quantity} pour la variante '{Sku}'.");

        return new Money(tier.UnitPriceHT) * CalculatePrintCoefficient();
    }

    /// <summary>
    /// Calcule le coefficient multiplicateur lié aux options d'impression.
    /// </summary>
    /// <returns>
    /// Produit du coefficient <see cref="PrintSide"/> × coefficient <see cref="ColorCount"/>.
    /// <list type="table">
    ///   <item>SingleSide=1.00, DoubleSide=1.15</item>
    ///   <item>One=1.00, Two=1.10, Three=1.18, FourCMYK=1.25</item>
    /// </list>
    /// </returns>
    public decimal CalculatePrintCoefficient()
    {
        decimal printSideCoeff = PrintSide switch
        {
            PrintSide.SingleSide => 1.00m,
            PrintSide.DoubleSide => 1.15m,
            _ => 1.00m
        };

        decimal colorCountCoeff = ColorCount switch
        {
            ColorCount.One     => 1.00m,
            ColorCount.Two     => 1.10m,
            ColorCount.Three   => 1.18m,
            ColorCount.FourCMYK => 1.25m,
            _ => 1.00m
        };

        return printSideCoeff * colorCountCoeff;
    }
}
