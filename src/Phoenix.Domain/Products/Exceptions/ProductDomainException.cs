namespace Phoenix.Domain.Products.Exceptions;

/// <summary>
/// Exception métier levée lorsqu'un invariant du domaine Product est violé.
/// Contient un <see cref="Code"/> machine-readable pour le traitement côté Application.
/// </summary>
public sealed class ProductDomainException : Exception
{
    // ── Codes d'erreur statiques ────────────────────────────────────────────

    /// <summary>Le SKU du produit ou de la variante est null ou vide.</summary>
    public const string SkuRequired = "PRODUCT_SKU_REQUIRED";

    /// <summary>Un produit ou une variante avec ce SKU existe déjà.</summary>
    public const string SkuAlreadyExists = "PRODUCT_SKU_ALREADY_EXISTS";

    /// <summary>La quantité commandée est inférieure au MOQ de la variante.</summary>
    public const string QuantityBelowMinimum = "PRODUCT_QUANTITY_BELOW_MINIMUM";

    /// <summary>
    /// La configuration du palier tarifaire est invalide
    /// (overlap, prix négatif, quantités incohérentes).
    /// </summary>
    public const string InvalidPriceTier = "PRODUCT_INVALID_PRICE_TIER";

    /// <summary>
    /// Le chemin de l'image est invalide (URL absolue au lieu de chemin relatif,
    /// image introuvable dans la collection du produit, etc.).
    /// </summary>
    public const string InvalidImagePath = "PRODUCT_INVALID_IMAGE_PATH";

    /// <summary>La variante recherchée n'existe pas sur ce produit.</summary>
    public const string VariantNotFound = "PRODUCT_VARIANT_NOT_FOUND";

    // ── Constructeur ────────────────────────────────────────────────────────

    /// <summary>
    /// Initialise une <see cref="ProductDomainException"/> avec un code et un message.
    /// </summary>
    /// <param name="code">
    /// Code machine-readable (utiliser les constantes de cette classe,
    /// ex : <see cref="SkuRequired"/>).
    /// </param>
    /// <param name="message">Message lisible par un développeur (logs, debug).</param>
    public ProductDomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Code machine-readable identifiant la règle métier violée.
    /// Mappé vers un <c>ProblemDetails</c> dans la couche API.
    /// </summary>
    public string Code { get; }
}
