namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Résultat du traitement d'une image produit.
/// Contient les flux WebP de l'image principale et de la miniature, prêts à être uploadés.
/// </summary>
/// <param name="MainStream">Flux WebP de l'image principale (800 px max, qualité 85). Position à 0.</param>
/// <param name="MainFileName">Nom de fichier de l'image principale (ex : "main-1700000000.webp").</param>
/// <param name="ThumbStream">Flux WebP de la miniature (400 px max, qualité 80). Position à 0.</param>
/// <param name="ThumbFileName">Nom de fichier de la miniature (ex : "thumb-1700000000.webp").</param>
/// <param name="ContentType">Type MIME commun : toujours "image/webp".</param>
public record ProcessedImage(
    Stream MainStream,
    string MainFileName,
    Stream ThumbStream,
    string ThumbFileName,
    string ContentType);

/// <summary>
/// Port (interface) du service de traitement d'images.
/// L'implémentation concrète réside dans Phoenix.Infrastructure et utilise SkiaSharp 3.
/// Aucune dépendance NuGet dans le Domain — uniquement ce contrat et les types BCL.
/// </summary>
public interface IImageProcessingService
{
    /// <summary>
    /// Traite une image produit en entrée et produit deux variantes WebP :
    /// une image principale (800 px max) et une miniature (400 px max).
    /// </summary>
    /// <param name="inputStream">Flux binaire de l'image source (jpg, jpeg, png ou webp).</param>
    /// <param name="originalFileName">Nom de fichier original, utilisé pour valider l'extension.</param>
    /// <param name="ct">Jeton d'annulation.</param>
    /// <returns>
    /// <see cref="ProcessedImage"/> contenant les deux flux WebP et leurs noms de fichier horodatés.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Levée si l'extension du fichier n'est pas supportée (jpg, jpeg, png, webp uniquement)
    /// ou si le décodage de l'image échoue.
    /// </exception>
    Task<ProcessedImage> ProcessProductImageAsync(
        Stream inputStream,
        string originalFileName,
        CancellationToken ct = default);
}
