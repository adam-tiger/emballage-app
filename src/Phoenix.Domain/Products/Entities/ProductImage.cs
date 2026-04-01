using Phoenix.Domain.Common.Primitives;

namespace Phoenix.Domain.Catalog.Entities;

/// <summary>
/// Image associée à un produit, stockée dans le blob storage Azure.
/// </summary>
/// <remarks>
/// <b>Invariants métier :</b>
/// <list type="bullet">
///   <item><see cref="BlobPath"/> est un chemin relatif dans le conteneur (jamais une URL absolue).</item>
///   <item><see cref="PublicUrl"/> est l'URL CDN publique calculée par <c>IBlobStorageService.GetPublicUrl</c>.</item>
///   <item>Un seul enregistrement peut avoir <see cref="IsMain"/> = <c>true</c> pour un produit donné
///         (garanti par l'agrégat <see cref="Aggregates.Product"/>).</item>
/// </list>
/// </remarks>
public sealed class ProductImage : Entity
{
    /// <summary>
    /// Constructeur sans paramètre réservé à EF Core.
    /// </summary>
    private ProductImage() { }

    private ProductImage(
        Guid id,
        Guid productId,
        string blobPath,
        string publicUrl,
        bool isMain,
        int sortOrder)
        : base(id)
    {
        ProductId = productId;
        BlobPath = blobPath;
        PublicUrl = publicUrl;
        IsMain = isMain;
        SortOrder = sortOrder;
    }

    /// <summary>Identifiant du produit auquel cette image appartient.</summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Chemin relatif dans le conteneur blob (ex : "products/images/abc123/front.jpg").
    /// C'est cette valeur qui est persistée en base et utilisée pour les opérations blob.
    /// </summary>
    public string BlobPath { get; private set; } = string.Empty;

    /// <summary>
    /// URL CDN publique calculée à partir du <see cref="BlobPath"/>
    /// (ex : "https://cdn.phoenix.fr/products/images/abc123/front.jpg").
    /// Mise à jour automatiquement si la configuration CDN change.
    /// </summary>
    public string PublicUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Indique si cette image est l'image principale du produit (vignette catalogue).
    /// Un seul <c>true</c> autorisé par produit.
    /// </summary>
    public bool IsMain { get; private set; }

    /// <summary>
    /// Ordre d'affichage dans la galerie produit (0 = premier).
    /// </summary>
    public int SortOrder { get; private set; }

    // ----- Factory method -----

    /// <summary>
    /// Crée une image produit après validation des invariants.
    /// </summary>
    /// <param name="productId">Identifiant du produit parent.</param>
    /// <param name="blobPath">Chemin relatif dans le conteneur blob (non vide).</param>
    /// <param name="publicUrl">URL CDN publique (non vide).</param>
    /// <param name="isMain">Image principale ?</param>
    /// <param name="sortOrder">Ordre d'affichage (>= 0).</param>
    public static ProductImage Create(
        Guid productId,
        string blobPath,
        string publicUrl,
        bool isMain,
        int sortOrder = 0)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Le productId ne peut pas être vide.", nameof(productId));

        if (string.IsNullOrWhiteSpace(blobPath))
            throw new ArgumentException("Le chemin blob ne peut pas être vide.", nameof(blobPath));

        if (string.IsNullOrWhiteSpace(publicUrl))
            throw new ArgumentException("L'URL publique ne peut pas être vide.", nameof(publicUrl));

        // BlobPath est relatif, jamais une URL absolue
        if (blobPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            blobPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(
                "BlobPath doit être un chemin relatif, pas une URL absolue.", nameof(blobPath));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(sortOrder),
                $"L'ordre de tri doit être >= 0 (valeur : {sortOrder}).");

        return new ProductImage(
            id: Guid.CreateVersion7(),
            productId: productId,
            blobPath: blobPath.Trim(),
            publicUrl: publicUrl.Trim(),
            isMain: isMain,
            sortOrder: sortOrder);
    }

    // ----- Méthodes métier -----

    /// <summary>
    /// Marque cette image comme principale. Appelé exclusivement par l'agrégat Product
    /// qui garantit l'unicité du flag <c>IsMain</c>.
    /// </summary>
    internal void SetAsMain() => IsMain = true;

    /// <summary>
    /// Retire le statut d'image principale. Appelé exclusivement par l'agrégat Product.
    /// </summary>
    internal void UnsetAsMain() => IsMain = false;

    /// <summary>
    /// Met à jour l'URL publique CDN (ex : lors d'un changement de domaine CDN).
    /// </summary>
    public void UpdatePublicUrl(string newPublicUrl)
    {
        if (string.IsNullOrWhiteSpace(newPublicUrl))
            throw new ArgumentException("L'URL publique ne peut pas être vide.", nameof(newPublicUrl));

        PublicUrl = newPublicUrl.Trim();
    }

    /// <summary>
    /// Met à jour l'ordre d'affichage de l'image.
    /// </summary>
    public void UpdateSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(sortOrder),
                $"L'ordre de tri doit être >= 0 (valeur : {sortOrder}).");

        SortOrder = sortOrder;
    }
}
