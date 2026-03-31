namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Port (interface) du service de stockage d'objets binaires (Azure Blob Storage, S3, MinIO…).
/// L'implémentation concrète réside dans Phoenix.Infrastructure.
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Charge un flux binaire dans le blob storage.
    /// </summary>
    /// <param name="content">Flux du fichier à uploader.</param>
    /// <param name="blobPath">Chemin relatif dans le conteneur (ex : "products/images/abc.jpg").</param>
    /// <param name="contentType">MIME type du fichier (ex : "image/jpeg").</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Résultat contenant le chemin relatif et l'URL CDN publique.</returns>
    Task<BlobUploadResult> UploadAsync(
        Stream content,
        string blobPath,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un blob à partir de son chemin relatif.
    /// Ne lève pas d'exception si le blob n'existe pas.
    /// </summary>
    Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Télécharge un blob sous forme de flux.
    /// </summary>
    Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie l'existence d'un blob sans le télécharger.
    /// </summary>
    Task<bool> ExistsAsync(string blobPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renvoie l'URL CDN publique pour un chemin relatif donné.
    /// Aucun appel réseau n'est effectué (calcul local).
    /// </summary>
    string GetPublicUrl(string blobPath);
}

/// <summary>
/// Résultat d'un upload réussi vers le blob storage.
/// </summary>
/// <param name="BlobPath">Chemin relatif dans le conteneur (persisté en base).</param>
/// <param name="PublicUrl">URL CDN publique prête à être servie aux clients.</param>
/// <param name="SizeInBytes">Taille du fichier uploadé en octets.</param>
/// <param name="ContentType">MIME type du fichier uploadé.</param>
public record BlobUploadResult(
    string BlobPath,
    string PublicUrl,
    long SizeInBytes,
    string ContentType);
