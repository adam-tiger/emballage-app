using Phoenix.Domain.Catalog.ValueObjects;
using Phoenix.Domain.Common.Primitives;

namespace Phoenix.Domain.Catalog.Entities;

/// <summary>
/// Variante d'un produit du catalogue (déclinaison impression).
/// Une variante est caractérisée par sa face d'impression, son nombre de couleurs
/// et la quantité minimale de commande (MOQ).
/// </summary>
/// <remarks>
/// <b>Invariants métier :</b>
/// <list type="bullet">
///   <item><see cref="Moq"/> >= 1 (on ne peut pas commander 0 unité).</item>
///   <item><see cref="Reference"/> est unique au sein du catalogue (vérifié au niveau Application).</item>
/// </list>
/// </remarks>
public sealed class ProductVariant : Entity
{
    /// <summary>
    /// Constructeur sans paramètre réservé à EF Core.
    /// </summary>
    private ProductVariant() { }

    private ProductVariant(
        Guid id,
        Guid productId,
        string reference,
        string name,
        int moq,
        PrintSide printSide,
        ColorCount colorCount)
        : base(id)
    {
        ProductId = productId;
        Reference = reference;
        Name = name;
        Moq = moq;
        PrintSide = printSide;
        ColorCount = colorCount;
    }

    /// <summary>Identifiant du produit auquel cette variante appartient.</summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Référence SKU unique de la variante (ex : "SAC-KRAFT-1C-R").
    /// Utilisée dans les commandes et les exports EDI.
    /// </summary>
    public string Reference { get; private set; } = string.Empty;

    /// <summary>Libellé commercial de la variante.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Minimum Order Quantity — quantité minimale de commande pour cette variante.
    /// Doit être >= 1.
    /// </summary>
    public int Moq { get; private set; }

    /// <summary>
    /// Face(s) imprimée(s) : recto uniquement ou recto-verso.
    /// </summary>
    /// <remarks>
    /// Stocké en PostgreSQL en tant que <c>varchar</c> via HasConversion&lt;string&gt;().
    /// </remarks>
    public PrintSide PrintSide { get; private set; }

    /// <summary>
    /// Nombre de couleurs d'impression (1, 2, 3 ou 4 CMJN).
    /// </summary>
    /// <remarks>
    /// Stocké en PostgreSQL en tant que <c>varchar</c> via HasConversion&lt;string&gt;().
    /// </remarks>
    public ColorCount ColorCount { get; private set; }

    // ----- Factory method -----

    /// <summary>
    /// Crée une variante de produit après validation des invariants.
    /// </summary>
    /// <param name="productId">Identifiant du produit parent.</param>
    /// <param name="reference">Référence SKU de la variante (non vide).</param>
    /// <param name="name">Libellé commercial (non vide).</param>
    /// <param name="moq">Quantité minimale de commande (>= 1).</param>
    /// <param name="printSide">Face(s) imprimée(s).</param>
    /// <param name="colorCount">Nombre de couleurs.</param>
    public static ProductVariant Create(
        Guid productId,
        string reference,
        string name,
        int moq,
        PrintSide printSide,
        ColorCount colorCount)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Le productId ne peut pas être vide.", nameof(productId));

        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("La référence SKU ne peut pas être vide.", nameof(reference));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le libellé de la variante ne peut pas être vide.", nameof(name));

        if (moq < 1)
            throw new ArgumentOutOfRangeException(nameof(moq),
                $"Le MOQ doit être >= 1 (valeur : {moq}).");

        if (!Enum.IsDefined(printSide))
            throw new ArgumentOutOfRangeException(nameof(printSide), $"PrintSide invalide : {printSide}.");

        if (!Enum.IsDefined(colorCount))
            throw new ArgumentOutOfRangeException(nameof(colorCount), $"ColorCount invalide : {colorCount}.");

        return new ProductVariant(
            id: Guid.CreateVersion7(),
            productId: productId,
            reference: reference.Trim(),
            name: name.Trim(),
            moq: moq,
            printSide: printSide,
            colorCount: colorCount);
    }

    // ----- Méthodes métier -----

    /// <summary>
    /// Met à jour le MOQ. La nouvelle valeur doit être >= 1.
    /// </summary>
    public void UpdateMoq(int newMoq)
    {
        if (newMoq < 1)
            throw new ArgumentOutOfRangeException(nameof(newMoq),
                $"Le MOQ doit être >= 1 (valeur : {newMoq}).");

        Moq = newMoq;
    }

    /// <summary>
    /// Met à jour les paramètres d'impression de la variante.
    /// </summary>
    public void UpdatePrintOptions(PrintSide printSide, ColorCount colorCount)
    {
        if (!Enum.IsDefined(printSide))
            throw new ArgumentOutOfRangeException(nameof(printSide), $"PrintSide invalide : {printSide}.");

        if (!Enum.IsDefined(colorCount))
            throw new ArgumentOutOfRangeException(nameof(colorCount), $"ColorCount invalide : {colorCount}.");

        PrintSide = printSide;
        ColorCount = colorCount;
    }
}
