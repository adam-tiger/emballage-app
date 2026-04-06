namespace Phoenix.Domain.Customizations.Events;

/// <summary>
/// Événement de domaine émis lorsqu'un logo client est uploadé dans un
/// <c>CustomizationJob</c> via <c>CustomizationJob.UploadLogo()</c>.
/// Peut déclencher un traitement asynchrone du fichier (vérification format,
/// conversion WebP via SkiaSharp, mise en cache CDN).
/// </summary>
/// <param name="JobId">Identifiant unique du job de personnalisation concerné.</param>
/// <param name="LogoFilePath">
/// Chemin relatif du logo dans le Blob Storage Azure
/// (ex : <c>logos/{jobId}/original.svg</c>).
/// </param>
/// <param name="OccurredAtUtc">Horodatage UTC de l'événement.</param>
public record LogoUploadedEvent(
    Guid     JobId,
    string   LogoFilePath,
    DateTime OccurredAtUtc);
