namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Résultat d'un upload réussi vers le blob storage.
/// Contient les chemins relatifs et URLs publiques CDN de l'image et de sa miniature.
/// </summary>
/// <param name="BlobPath">Chemin RELATIF dans le conteneur (persisté en base, ex : "products/abc/main.webp").</param>
/// <param name="PublicUrl">URL CDN complète prête à servir aux clients.</param>
/// <param name="ThumbBlobPath">Chemin relatif de la miniature (null si non générée).</param>
/// <param name="ThumbPublicUrl">URL CDN de la miniature (null si non générée).</param>
public record BlobUploadResult(
    string BlobPath,
    string PublicUrl,
    string? ThumbBlobPath,
    string? ThumbPublicUrl);

/// <summary>
/// Port (interface) du service de stockage d'objets binaires.
/// L'implémentation concrète réside dans Phoenix.Infrastructure (Azure Blob Storage).
/// Zéro dépendance NuGet dans le Domain — uniquement ce contrat.
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Upload une image produit et génère automatiquement une miniature.
    /// Le chemin de stockage est construit à partir du <paramref name="productId"/>.
    /// </summary>
    /// <param name="productId">Identifiant du produit propriétaire de l'image.</param>
    /// <param name="stream">Flux binaire du fichier image.</param>
    /// <param name="fileName">Nom de fichier original (ex : "front.webp").</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>
    /// <see cref="BlobUploadResult"/> contenant BlobPath, PublicUrl et les infos miniature.
    /// </returns>
    Task<BlobUploadResult> UploadProductImageAsync(
        Guid productId,
        Stream stream,
        string fileName,
        CancellationToken ct = default);

    /// <summary>
    /// Upload le logo d'un client associé à un job de personnalisation.
    /// Chemin construit à partir de <paramref name="customerId"/> / <paramref name="jobId"/>.
    /// </summary>
    /// <param name="customerId">Identifiant du client.</param>
    /// <param name="jobId">Identifiant du job de personnalisation.</param>
    /// <param name="stream">Flux binaire du logo.</param>
    /// <param name="fileName">Nom de fichier original.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task<BlobUploadResult> UploadCustomerLogoAsync(
        Guid customerId,
        Guid jobId,
        Stream stream,
        string fileName,
        CancellationToken ct = default);

    /// <summary>
    /// Upload un document générique (bon de commande, CGV, devis, etc.).
    /// </summary>
    /// <param name="containerPath">Chemin relatif du répertoire cible dans le conteneur.</param>
    /// <param name="stream">Flux binaire du document.</param>
    /// <param name="fileName">Nom de fichier original.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task<BlobUploadResult> UploadDocumentAsync(
        string containerPath,
        Stream stream,
        string fileName,
        CancellationToken ct = default);

    /// <summary>
    /// Génère une URL SAS (Shared Access Signature) temporaire pour accès sécurisé à un blob privé.
    /// </summary>
    /// <param name="containerName">Nom du conteneur Azure Blob Storage.</param>
    /// <param name="blobPath">Chemin relatif du blob dans le conteneur.</param>
    /// <param name="duration">Durée de validité de l'URL SAS.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>URL SAS temporaire sous forme de chaîne.</returns>
    Task<string> GenerateSasUrlAsync(
        string containerName,
        string blobPath,
        TimeSpan duration,
        CancellationToken ct = default);

    /// <summary>
    /// Supprime un blob existant. Ne lève pas d'exception si le blob n'existe pas.
    /// </summary>
    /// <param name="containerName">Nom du conteneur Azure Blob Storage.</param>
    /// <param name="blobPath">Chemin relatif du blob à supprimer.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    Task DeleteAsync(
        string containerName,
        string blobPath,
        CancellationToken ct = default);
}
