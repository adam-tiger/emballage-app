using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Infrastructure.Storage;

/// <summary>
/// Implémentation de <see cref="IBlobStorageService"/> pour Azure Blob Storage (SDK v12).
/// Gère l'upload d'images produit (avec traitement SkiaSharp via <see cref="IImageProcessingService"/>),
/// de logos clients, de documents, la génération de SAS tokens et la suppression de blobs.
/// </summary>
/// <remarks>
/// <b>Conteneurs :</b>
/// <list type="table">
///   <item><c>phoenix-product-images</c> — accès public Blob (CDN)</item>
///   <item><c>phoenix-customer-logos</c> — accès privé (SAS à la demande)</item>
///   <item><c>phoenix-documents</c> — accès privé (SAS à la demande)</item>
/// </list>
/// </remarks>
internal sealed class AzureBlobStorageService(
    IOptions<BlobStorageSettings> options,
    IImageProcessingService imageProcessingService,
    ILogger<AzureBlobStorageService> logger) : IBlobStorageService
{
    private readonly BlobStorageSettings _settings = options.Value;
    private readonly BlobServiceClient _blobServiceClient = new BlobServiceClient(options.Value.ConnectionString);

    private static readonly HashSet<string> AllowedLogoExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".svg", ".pdf", ".png", ".ai" };

    // ── IBlobStorageService ──────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<BlobUploadResult> UploadProductImageAsync(
        Guid productId,
        Stream stream,
        string fileName,
        CancellationToken ct = default)
    {
        try
        {
            // ── 1. Traitement SkiaSharp ──────────────────────────────────────
            var processed = await imageProcessingService
                .ProcessProductImageAsync(stream, fileName, ct);

            var container = await GetOrCreateContainerAsync(
                _settings.ProductImagesContainer, PublicAccessType.Blob, ct);

            // ── 2. Upload image principale ───────────────────────────────────
            var mainBlobPath = _settings.BuildProductImagePath(productId, processed.MainFileName);
            var mainClient = container.GetBlobClient(mainBlobPath);

            processed.MainStream.Position = 0;
            await mainClient.UploadAsync(
                processed.MainStream,
                new BlobHttpHeaders { ContentType = processed.ContentType },
                conditions: null,
                cancellationToken: ct);

            // ── 3. Upload miniature ──────────────────────────────────────────
            var thumbBlobPath = _settings.BuildProductImagePath(productId, processed.ThumbFileName);
            var thumbClient = container.GetBlobClient(thumbBlobPath);

            processed.ThumbStream.Position = 0;
            await thumbClient.UploadAsync(
                processed.ThumbStream,
                new BlobHttpHeaders { ContentType = processed.ContentType },
                conditions: null,
                cancellationToken: ct);

            logger.LogInformation(
                "Product image uploaded. ProductId: {ProductId}, BlobPath: {BlobPath}",
                productId, mainBlobPath);

            // ── 4. Résultat ──────────────────────────────────────────────────
            return new BlobUploadResult(
                BlobPath:       mainBlobPath,
                PublicUrl:      _settings.BuildPublicUrl(mainBlobPath),
                ThumbBlobPath:  thumbBlobPath,
                ThumbPublicUrl: _settings.BuildPublicUrl(thumbBlobPath));
        }
        catch (InvalidOperationException)
        {
            throw; // déjà un message métier explicite depuis ImageProcessingService
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erreur lors de l'upload de l'image produit. ProductId: {ProductId}, FileName: {FileName}",
                productId, fileName);
            throw new InvalidOperationException(
                $"L'upload de l'image produit '{fileName}' a échoué.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<BlobUploadResult> UploadCustomerLogoAsync(
        Guid customerId,
        Guid jobId,
        Stream stream,
        string fileName,
        CancellationToken ct = default)
    {
        try
        {
            // ── 1. Validation de l'extension ─────────────────────────────────
            var extension = Path.GetExtension(fileName);
            if (!AllowedLogoExtensions.Contains(extension))
                throw new InvalidOperationException(
                    $"Format de logo non supporté : '{extension}'. " +
                    $"Formats acceptés : {string.Join(", ", AllowedLogoExtensions)}.");

            // ── 2. Nom de fichier sécurisé horodaté ──────────────────────────
            var safeFileName = $"original-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";
            var blobPath = _settings.BuildCustomerLogoPath(customerId, jobId, safeFileName);

            // ── 3. Upload (conteneur privé — pas de CDN) ─────────────────────
            var container = await GetOrCreateContainerAsync(
                _settings.CustomerLogosContainer, PublicAccessType.None, ct);

            var blobClient = container.GetBlobClient(blobPath);
            var contentType = ResolveLogoContentType(extension);

            stream.Position = 0;
            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                conditions: null,
                cancellationToken: ct);

            logger.LogInformation(
                "Customer logo uploaded. CustomerId: {CustomerId}, JobId: {JobId}, BlobPath: {BlobPath}",
                customerId, jobId, blobPath);

            // ── 4. PublicUrl = blobPath relatif (conteneur privé — SAS requis) ─
            return new BlobUploadResult(
                BlobPath:       blobPath,
                PublicUrl:      blobPath,
                ThumbBlobPath:  null,
                ThumbPublicUrl: null);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erreur lors de l'upload du logo client. CustomerId: {CustomerId}, JobId: {JobId}, FileName: {FileName}",
                customerId, jobId, fileName);
            throw new InvalidOperationException(
                $"L'upload du logo '{fileName}' a échoué.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<BlobUploadResult> UploadDocumentAsync(
        string containerPath,
        Stream stream,
        string fileName,
        CancellationToken ct = default)
    {
        try
        {
            var blobPath = _settings.BuildDocumentPath(containerPath, fileName);

            var container = await GetOrCreateContainerAsync(
                _settings.DocumentsContainer, PublicAccessType.None, ct);

            var blobClient = container.GetBlobClient(blobPath);

            stream.Position = 0;
            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = "application/pdf" },
                conditions: null,
                cancellationToken: ct);

            logger.LogInformation(
                "Document uploaded. BlobPath: {BlobPath}",
                blobPath);

            return new BlobUploadResult(
                BlobPath:       blobPath,
                PublicUrl:      blobPath,
                ThumbBlobPath:  null,
                ThumbPublicUrl: null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erreur lors de l'upload du document. ContainerPath: {ContainerPath}, FileName: {FileName}",
                containerPath, fileName);
            throw new InvalidOperationException(
                $"L'upload du document '{fileName}' a échoué.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateSasUrlAsync(
        string containerName,
        string blobPath,
        TimeSpan duration,
        CancellationToken ct = default)
    {
        try
        {
            var container  = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = container.GetBlobClient(blobPath);

            // Vérifie l'existence du blob avant de générer le SAS
            var exists = await blobClient.ExistsAsync(ct);
            if (!exists.Value)
                throw new FileNotFoundException(
                    $"Le blob '{blobPath}' n'existe pas dans le conteneur '{containerName}'.");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName          = blobPath,
                Resource          = "b",
                ExpiresOn         = DateTimeOffset.UtcNow.Add(duration)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder).ToString();

            logger.LogInformation(
                "SAS URL générée. Container: {Container}, BlobPath: {BlobPath}, Duration: {Duration}",
                containerName, blobPath, duration);

            return sasUri;
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erreur lors de la génération du SAS. Container: {Container}, BlobPath: {BlobPath}",
                containerName, blobPath);
            throw new InvalidOperationException(
                $"La génération du SAS pour '{blobPath}' a échoué.", ex);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        string containerName,
        string blobPath,
        CancellationToken ct = default)
    {
        try
        {
            var container  = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = container.GetBlobClient(blobPath);

            await blobClient.DeleteIfExistsAsync(cancellationToken: ct);

            logger.LogInformation(
                "Blob supprimé. Container: {Container}, BlobPath: {BlobPath}",
                containerName, blobPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erreur lors de la suppression du blob. Container: {Container}, BlobPath: {BlobPath}",
                containerName, blobPath);
            throw new InvalidOperationException(
                $"La suppression du blob '{blobPath}' dans '{containerName}' a échoué.", ex);
        }
    }

    // ── Helpers privés ───────────────────────────────────────────────────────

    /// <summary>
    /// Récupère ou crée un conteneur Azure Blob avec le niveau d'accès spécifié.
    /// Opération idempotente — sans effet si le conteneur existe déjà.
    /// </summary>
    /// <param name="containerName">Nom du conteneur.</param>
    /// <param name="accessType">Niveau d'accès public : <see cref="PublicAccessType.Blob"/> ou <see cref="PublicAccessType.None"/>.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    private async Task<BlobContainerClient> GetOrCreateContainerAsync(
        string containerName,
        PublicAccessType accessType,
        CancellationToken ct)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(accessType, cancellationToken: ct);
        return container;
    }

    /// <summary>
    /// Résout le Content-Type MIME d'un logo selon son extension.
    /// </summary>
    private static string ResolveLogoContentType(string extension) =>
        extension.ToLowerInvariant() switch
        {
            ".svg" => "image/svg+xml",
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".ai"  => "application/postscript",
            _      => "application/octet-stream"
        };
}
