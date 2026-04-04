namespace Phoenix.Infrastructure.Storage;

/// <summary>
/// Configuration du service Azure Blob Storage.
/// Liée depuis <c>appsettings.json</c> section <c>Azure:BlobStorage</c> via
/// <c>services.Configure&lt;BlobStorageSettings&gt;(configuration.GetSection("Azure:BlobStorage"))</c>.
/// </summary>
public sealed class BlobStorageSettings
{
    /// <summary>Chaîne de connexion Azure Storage (Key, SAS ou "UseDevelopmentStorage=true" pour Azurite).</summary>
    public required string ConnectionString { get; init; }

    /// <summary>Nom du conteneur des images produits. Défaut : "phoenix-product-images".</summary>
    public required string ProductImagesContainer { get; init; } = "phoenix-product-images";

    /// <summary>Nom du conteneur des logos clients. Défaut : "phoenix-customer-logos".</summary>
    public required string CustomerLogosContainer { get; init; } = "phoenix-customer-logos";

    /// <summary>Nom du conteneur des documents (bons de commande, CGV, devis…). Défaut : "phoenix-documents".</summary>
    public required string DocumentsContainer { get; init; } = "phoenix-documents";

    /// <summary>
    /// URL de base du CDN Azure Front Door ou Azurite en local.
    /// Défaut production : "https://cdn.phoenix-emballages.fr".
    /// Défaut local : "http://127.0.0.1:10000/devstoreaccount1".
    /// </summary>
    public required string CdnBaseUrl { get; init; } = "https://cdn.phoenix-emballages.fr";

    // ── Helpers de construction de chemin ────────────────────────────────────

    /// <summary>
    /// Construit l'URL CDN publique complète à partir d'un chemin blob relatif.
    /// </summary>
    /// <param name="blobPath">Chemin relatif dans le conteneur (ex : "products/abc/main.webp").</param>
    /// <returns>URL complète (ex : "https://cdn.phoenix-emballages.fr/products/abc/main.webp").</returns>
    public string BuildPublicUrl(string blobPath) =>
        $"{CdnBaseUrl.TrimEnd('/')}/{blobPath.TrimStart('/')}";

    /// <summary>
    /// Construit le chemin blob relatif pour une image produit.
    /// Format : <c>products/{productId}/{fileName}</c>.
    /// </summary>
    /// <param name="productId">Identifiant du produit propriétaire.</param>
    /// <param name="fileName">Nom de fichier (ex : "main-1700000000.webp").</param>
    public string BuildProductImagePath(Guid productId, string fileName) =>
        $"products/{productId}/{fileName}";

    /// <summary>
    /// Construit le chemin blob relatif pour un logo client.
    /// Format : <c>customers/{customerId}/jobs/{jobId}/{fileName}</c>.
    /// </summary>
    /// <param name="customerId">Identifiant du client.</param>
    /// <param name="jobId">Identifiant du job de personnalisation.</param>
    /// <param name="fileName">Nom de fichier sécurisé horodaté.</param>
    public string BuildCustomerLogoPath(Guid customerId, Guid jobId, string fileName) =>
        $"customers/{customerId}/jobs/{jobId}/{fileName}";

    /// <summary>
    /// Construit le chemin blob relatif pour un document générique.
    /// Format : <c>{containerPath}/{fileName}</c> (slashes normalisés).
    /// </summary>
    /// <param name="containerPath">Répertoire cible relatif (ex : "orders/2025").</param>
    /// <param name="fileName">Nom de fichier du document.</param>
    public string BuildDocumentPath(string containerPath, string fileName) =>
        $"{containerPath.Trim('/')}/{fileName}";
}
