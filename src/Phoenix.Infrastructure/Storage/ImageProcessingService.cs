using Phoenix.Domain.Common.Interfaces;
using SkiaSharp;

namespace Phoenix.Infrastructure.Storage;

/// <summary>
/// Implémentation de <see cref="IImageProcessingService"/> utilisant SkiaSharp 3.
/// Traite les images produit en deux variantes WebP : principale (800 px) et miniature (400 px).
/// </summary>
/// <remarks>
/// <b>Formats acceptés :</b> jpg, jpeg, png, webp.
/// <b>Format de sortie :</b> WebP (qualité 85 pour le principal, 80 pour la miniature).
/// <b>Redimensionnement :</b> proportionnel, basé sur le côté le plus long.
/// <b>Compatibilité Linux :</b> SkiaSharp — System.Drawing est proscrit.
/// </remarks>
internal sealed class ImageProcessingService : IImageProcessingService
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    /// <inheritdoc />
    public Task<ProcessedImage> ProcessProductImageAsync(
        Stream inputStream,
        string originalFileName,
        CancellationToken ct = default)
    {
        // ── 1. Validation du format ──────────────────────────────────────────
        var extension = Path.GetExtension(originalFileName);
        if (!SupportedExtensions.Contains(extension))
            throw new InvalidOperationException(
                $"Format d'image non supporté : '{extension}'. " +
                $"Formats acceptés : {string.Join(", ", SupportedExtensions)}.");

        // ── 2. Décodage ──────────────────────────────────────────────────────
        using var original = SKBitmap.Decode(inputStream);
        if (original is null)
            throw new InvalidOperationException(
                $"Impossible de décoder l'image '{originalFileName}'. " +
                "Le fichier est peut-être corrompu ou dans un format non reconnu.");

        // ── 3. Horodatage pour les noms de fichier ───────────────────────────
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // ── 4. Image principale (800 px max, qualité 85) ─────────────────────
        var mainBitmap = ResizeImage(original, 800);
        MemoryStream mainStream;
        try
        {
            mainStream = EncodeToWebP(mainBitmap, 85);
        }
        finally
        {
            if (!ReferenceEquals(mainBitmap, original))
                mainBitmap.Dispose();
        }

        // ── 5. Miniature (400 px max, qualité 80) ────────────────────────────
        var thumbBitmap = ResizeImage(original, 400);
        MemoryStream thumbStream;
        try
        {
            thumbStream = EncodeToWebP(thumbBitmap, 80);
        }
        finally
        {
            if (!ReferenceEquals(thumbBitmap, original))
                thumbBitmap.Dispose();
        }

        // ── 6. Résultat ──────────────────────────────────────────────────────
        var result = new ProcessedImage(
            MainStream:    mainStream,
            MainFileName:  $"main-{timestamp}.webp",
            ThumbStream:   thumbStream,
            ThumbFileName: $"thumb-{timestamp}.webp",
            ContentType:   "image/webp");

        return Task.FromResult(result);
    }

    // ── Helpers privés ───────────────────────────────────────────────────────

    /// <summary>
    /// Redimensionne un bitmap en conservant le ratio.
    /// Si les deux dimensions sont déjà inférieures ou égales à <paramref name="maxDimension"/>,
    /// retourne l'instance d'origine sans copie.
    /// </summary>
    /// <param name="original">Bitmap source.</param>
    /// <param name="maxDimension">Taille maximale (côté le plus long) en pixels.</param>
    /// <returns>
    /// Nouveau <see cref="SKBitmap"/> redimensionné, ou l'<paramref name="original"/> si inchangé.
    /// L'appelant est responsable de la libération du bitmap retourné si différent de l'original.
    /// </returns>
    private static SKBitmap ResizeImage(SKBitmap original, int maxDimension)
    {
        if (original.Width <= maxDimension && original.Height <= maxDimension)
            return original;

        var factor = maxDimension / (float)Math.Max(original.Width, original.Height);
        var newWidth  = (int)(original.Width  * factor);
        var newHeight = (int)(original.Height * factor);

        var info = new SKImageInfo(newWidth, newHeight, original.ColorType, original.AlphaType);
        var resizedBitmap = new SKBitmap(info);

        using var canvas = new SKCanvas(resizedBitmap);
        canvas.Clear(SKColors.Transparent);

        // Utiliser SKImage pour le redimensionnement avec SKSamplingOptions
        using var image = SKImage.FromBitmap(original);
        var destRect = SKRect.Create(0, 0, newWidth, newHeight);
        var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

        canvas.DrawImage(image, destRect, samplingOptions);

        return resizedBitmap;
    }

    /// <summary>
    /// Encode un bitmap en WebP et retourne un <see cref="MemoryStream"/> positionné à 0.
    /// </summary>
    /// <param name="bitmap">Bitmap à encoder.</param>
    /// <param name="quality">Qualité WebP (0–100).</param>
    private static MemoryStream EncodeToWebP(SKBitmap bitmap, int quality)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data  = image.Encode(SKEncodedImageFormat.Webp, quality);

        var stream = new MemoryStream();
        data.SaveTo(stream);
        stream.Position = 0;
        return stream;
    }
}
