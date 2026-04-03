namespace Phoenix.Domain.Products.Entities;

/// <summary>
/// Image associée à un produit, stockée dans Azure Blob Storage.
/// Contient le chemin relatif persisté en base et l'URL CDN publique.
/// </summary>
/// <remarks>
/// <b>Invariants métier :</b>
/// <list type="bullet">
///   <item><see cref="BlobPath"/> est un chemin RELATIF dans le conteneur (jamais une URL absolue).</item>
///   <item><see cref="PublicUrl"/> est l'URL CDN complète calculée par <c>IBlobStorageService</c>.</item>
///   <item>Un seul enregistrement peut avoir <see cref="IsMain"/> = <c>true</c> par produit
///         — garanti par l'agrégat <see cref="Product"/>.</item>
/// </list>
/// </remarks>
public sealed class ProductImage
{
    /// <summary>
    /// Constructeur public pour créer une image produit.
    /// Toutes les propriétés sont validées à la construction.
    /// </summary>
    /// <param name="id">Identifiant unique de l'image.</param>
    /// <param name="productId">Identifiant du produit propriétaire.</param>
    /// <param name="blobPath">Chemin relatif dans le conteneur blob (ex : "products/abc/main.webp").</param>
    /// <param name="publicUrl">URL CDN complète (ex : "https://cdn.phoenix.fr/products/abc/main.webp").</param>
    /// <param name="isMain">Indique si cette image est la vignette principale.</param>
    /// <param name="createdAtUtc">Date de création UTC.</param>
    /// <param name="thumbBlobPath">Chemin relatif de la miniature (optionnel).</param>
    /// <param name="thumbPublicUrl">URL CDN de la miniature (optionnel).</param>
    public ProductImage(
        Guid id,
        Guid productId,
        string blobPath,
        string publicUrl,
        bool isMain,
        DateTime createdAtUtc,
        string? thumbBlobPath = null,
        string? thumbPublicUrl = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("L'identifiant de l'image ne peut pas être vide.", nameof(id));

        if (productId == Guid.Empty)
            throw new ArgumentException("L'identifiant du produit ne peut pas être vide.", nameof(productId));

        if (string.IsNullOrWhiteSpace(blobPath))
            throw new ArgumentException("Le chemin blob ne peut pas être vide.", nameof(blobPath));

        if (blobPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            blobPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(
                "BlobPath doit être un chemin RELATIF dans le conteneur, pas une URL absolue.",
                nameof(blobPath));

        if (string.IsNullOrWhiteSpace(publicUrl))
            throw new ArgumentException("L'URL publique CDN ne peut pas être vide.", nameof(publicUrl));

        Id = id;
        ProductId = productId;
        BlobPath = blobPath.Trim();
        PublicUrl = publicUrl.Trim();
        IsMain = isMain;
        CreatedAtUtc = createdAtUtc;
        ThumbBlobPath = thumbBlobPath?.Trim();
        ThumbPublicUrl = thumbPublicUrl?.Trim();
    }

    /// <summary>Identifiant unique de l'image.</summary>
    public Guid Id { get; private set; }

    /// <summary>Identifiant du produit propriétaire de cette image.</summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Chemin RELATIF dans le conteneur blob (ex : "products/abc/main.webp").
    /// C'est cette valeur qui est persistée en base de données.
    /// </summary>
    public string BlobPath { get; private set; }

    /// <summary>
    /// URL CDN publique complète (ex : "https://cdn.phoenix.fr/products/abc/main.webp").
    /// Recalculable depuis <see cref="BlobPath"/> si le domaine CDN change.
    /// </summary>
    public string PublicUrl { get; private set; }

    /// <summary>
    /// Chemin relatif de la miniature générée automatiquement au moment de l'upload.
    /// <c>null</c> si aucune miniature n'a été générée.
    /// </summary>
    public string? ThumbBlobPath { get; private set; }

    /// <summary>
    /// URL CDN publique de la miniature.
    /// <c>null</c> si aucune miniature n'a été générée.
    /// </summary>
    public string? ThumbPublicUrl { get; private set; }

    /// <summary>
    /// Indique si cette image est l'image principale du produit (vignette catalogue).
    /// Un seul <c>true</c> autorisé par produit — garanti par l'agrégat.
    /// </summary>
    public bool IsMain { get; private set; }

    /// <summary>Date et heure de création UTC.</summary>
    public DateTime CreatedAtUtc { get; private set; }

    // ── Méthodes métier ─────────────────────────────────────────────────────

    /// <summary>
    /// Marque cette image comme principale.
    /// Appelé exclusivement par <see cref="Product.SetMainImage"/> qui garantit l'unicité.
    /// </summary>
    public void SetAsMain() => IsMain = true;

    /// <summary>
    /// Retire le statut d'image principale.
    /// Appelé exclusivement par <see cref="Product.SetMainImage"/>.
    /// </summary>
    public void UnsetAsMain() => IsMain = false;
}
