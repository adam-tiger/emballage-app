using Phoenix.Domain.Catalog.ValueObjects;
using Phoenix.Domain.Common.Primitives;

namespace Phoenix.Domain.Catalog.Entities;

/// <summary>
/// Palier tarifaire d'un produit.
/// Un produit possède une liste ordonnée de paliers couvrant l'ensemble des quantités possibles.
/// </summary>
/// <remarks>
/// <b>Invariants métier :</b>
/// <list type="bullet">
///   <item><see cref="MinQuantity"/> >= 1.</item>
///   <item>Quand <see cref="MaxQuantity"/> est renseigné, il doit être > <see cref="MinQuantity"/>.</item>
///   <item><see cref="UnitPriceHT"/> doit être strictement positif.</item>
/// </list>
/// La cohérence inter-paliers (pas de chevauchement, pas de trou) est vérifiée
/// dans l'agrégat <see cref="Aggregates.Product"/>.
/// </remarks>
public sealed class PriceTier : Entity
{
    /// <summary>
    /// Constructeur sans paramètre réservé à EF Core.
    /// </summary>
    private PriceTier() { }

    private PriceTier(Guid id, Guid productId, int minQuantity, int? maxQuantity, Money unitPriceHT)
        : base(id)
    {
        ProductId = productId;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        UnitPriceHT = unitPriceHT;
    }

    /// <summary>Identifiant du produit auquel ce palier appartient.</summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Quantité minimale (incluse) à partir de laquelle ce palier s'applique.
    /// Doit être >= 1.
    /// </summary>
    public int MinQuantity { get; private set; }

    /// <summary>
    /// Quantité maximale (incluse) jusqu'à laquelle ce palier s'applique.
    /// <c>null</c> signifie « sans plafond » (dernier palier de la gamme).
    /// </summary>
    public int? MaxQuantity { get; private set; }

    /// <summary>
    /// Prix unitaire hors-taxes pour ce palier.
    /// Doit être strictement positif.
    /// </summary>
    public Money UnitPriceHT { get; private set; } = Money.Zero;

    // ----- Factory method -----

    /// <summary>
    /// Crée un palier tarifaire après validation des invariants.
    /// </summary>
    /// <param name="productId">Identifiant du produit parent.</param>
    /// <param name="minQuantity">Quantité minimale (>= 1).</param>
    /// <param name="maxQuantity">Quantité maximale (optionnelle, null = illimité).</param>
    /// <param name="unitPriceHT">Prix unitaire HT (doit être > 0).</param>
    public static PriceTier Create(Guid productId, int minQuantity, int? maxQuantity, Money unitPriceHT)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Le productId ne peut pas être vide.", nameof(productId));

        if (minQuantity < 1)
            throw new ArgumentOutOfRangeException(nameof(minQuantity),
                $"La quantité minimale doit être >= 1 (valeur : {minQuantity}).");

        if (maxQuantity.HasValue && maxQuantity.Value <= minQuantity)
            throw new ArgumentOutOfRangeException(nameof(maxQuantity),
                $"La quantité maximale ({maxQuantity}) doit être > quantité minimale ({minQuantity}).");

        if (unitPriceHT is null)
            throw new ArgumentNullException(nameof(unitPriceHT));

        if (unitPriceHT.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(unitPriceHT),
                $"Le prix unitaire HT doit être strictement positif (valeur : {unitPriceHT}).");

        return new PriceTier(
            id: Guid.CreateVersion7(),
            productId: productId,
            minQuantity: minQuantity,
            maxQuantity: maxQuantity,
            unitPriceHT: unitPriceHT);
    }

    // ----- Méthodes métier -----

    /// <summary>
    /// Vérifie si une quantité donnée tombe dans ce palier.
    /// </summary>
    public bool Covers(int quantity) =>
        quantity >= MinQuantity && (!MaxQuantity.HasValue || quantity <= MaxQuantity.Value);

    /// <summary>
    /// Met à jour le prix unitaire HT. Le nouveau prix doit être > 0.
    /// </summary>
    public void UpdatePrice(Money newUnitPriceHT)
    {
        if (newUnitPriceHT is null)
            throw new ArgumentNullException(nameof(newUnitPriceHT));

        if (newUnitPriceHT.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(newUnitPriceHT),
                $"Le prix unitaire HT doit être strictement positif (valeur : {newUnitPriceHT}).");

        UnitPriceHT = newUnitPriceHT;
    }
}
