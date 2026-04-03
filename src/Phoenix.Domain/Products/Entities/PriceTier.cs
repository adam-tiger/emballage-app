using Phoenix.Domain.Products.Exceptions;

namespace Phoenix.Domain.Products.Entities;

/// <summary>
/// Palier tarifaire associé à une variante de produit.
/// Définit le prix unitaire HT applicable pour une plage de quantités commandées.
/// </summary>
/// <remarks>
/// <b>Invariants métier :</b>
/// <list type="bullet">
///   <item><see cref="MinQuantity"/> >= 1.</item>
///   <item>Si <see cref="MaxQuantity"/> est renseigné, il doit être > <see cref="MinQuantity"/>.</item>
///   <item><see cref="UnitPriceHT"/> doit être strictement positif (&gt; 0).</item>
/// </list>
/// La non-collision entre paliers est vérifiée dans <see cref="ProductVariant.AddPriceTier"/>.
/// </remarks>
public sealed class PriceTier
{
    /// <summary>
    /// Constructeur public avec validation des invariants métier.
    /// </summary>
    /// <param name="id">Identifiant unique du palier.</param>
    /// <param name="productVariantId">Identifiant de la variante parente.</param>
    /// <param name="minQuantity">Quantité minimale inclusive (>= 1).</param>
    /// <param name="maxQuantity">Quantité maximale inclusive (null = illimité).</param>
    /// <param name="unitPriceHT">Prix unitaire HT en euros (doit être > 0).</param>
    public PriceTier(Guid id, Guid productVariantId, int minQuantity, int? maxQuantity, decimal unitPriceHT)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("L'identifiant du palier ne peut pas être vide.", nameof(id));

        if (productVariantId == Guid.Empty)
            throw new ArgumentException("L'identifiant de la variante ne peut pas être vide.", nameof(productVariantId));

        if (minQuantity < 1)
            throw new ProductDomainException(
                ProductDomainException.InvalidPriceTier,
                $"La quantité minimale doit être >= 1 (valeur : {minQuantity}).");

        if (maxQuantity.HasValue && maxQuantity.Value <= minQuantity)
            throw new ProductDomainException(
                ProductDomainException.InvalidPriceTier,
                $"La quantité maximale ({maxQuantity}) doit être > quantité minimale ({minQuantity}).");

        if (unitPriceHT <= 0)
            throw new ProductDomainException(
                ProductDomainException.InvalidPriceTier,
                $"Le prix unitaire HT doit être strictement positif (valeur : {unitPriceHT}).");

        Id = id;
        ProductVariantId = productVariantId;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        UnitPriceHT = unitPriceHT;
    }

    /// <summary>Identifiant unique du palier tarifaire.</summary>
    public Guid Id { get; private set; }

    /// <summary>Identifiant de la variante produit à laquelle ce palier est rattaché.</summary>
    public Guid ProductVariantId { get; private set; }

    /// <summary>
    /// Quantité minimale (incluse) à partir de laquelle ce palier s'applique.
    /// Doit être >= 1.
    /// </summary>
    public int MinQuantity { get; private set; }

    /// <summary>
    /// Quantité maximale (incluse) jusqu'à laquelle ce palier s'applique.
    /// <c>null</c> = sans plafond (dernier palier de la gamme).
    /// </summary>
    public int? MaxQuantity { get; private set; }

    /// <summary>
    /// Prix unitaire hors-taxes en euros pour ce palier.
    /// Doit être strictement positif.
    /// </summary>
    public decimal UnitPriceHT { get; private set; }

    // ── Méthodes métier ─────────────────────────────────────────────────────

    /// <summary>
    /// Vérifie si une quantité donnée est couverte par ce palier tarifaire.
    /// </summary>
    /// <param name="quantity">Quantité à tester.</param>
    /// <returns>
    /// <c>true</c> si <paramref name="quantity"/> >= <see cref="MinQuantity"/> et
    /// (<see cref="MaxQuantity"/> est null ou <paramref name="quantity"/> &lt;= <see cref="MaxQuantity"/>).
    /// </returns>
    public bool Matches(int quantity) =>
        quantity >= MinQuantity && (MaxQuantity == null || quantity <= MaxQuantity.Value);
}
