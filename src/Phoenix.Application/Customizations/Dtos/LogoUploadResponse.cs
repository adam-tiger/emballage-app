namespace Phoenix.Application.Customizations.Dtos;

/// <summary>
/// Réponse retournée après un upload réussi du logo client.
/// Contient le chemin de stockage et une URL SAS temporaire (1h)
/// pour prévisualiser le logo dans le configurateur Angular / Konva.js.
/// </summary>
/// <param name="JobId">Identifiant du job de personnalisation mis à jour.</param>
/// <param name="LogoFilePath">
/// Chemin relatif du logo dans le Blob Storage Azure
/// (ex : <c>logos/{customerId}/{jobId}/mon-logo.svg</c>).
/// Persisté en base via <c>CustomizationJob.LogoFilePath</c>.
/// </param>
/// <param name="LogoSasUrl">
/// URL SAS temporaire (valide 1h) permettant à Angular d'afficher le logo
/// directement depuis Azure Blob Storage sans exposer les credentials.
/// Non persistée — régénérée à chaque requête.
/// </param>
/// <param name="LogoFileName">Nom de fichier original du logo uploadé.</param>
/// <param name="Status">Nouveau statut du job après upload (<c>"LogoUploaded"</c>).</param>
public sealed record LogoUploadResponse(
    Guid   JobId,
    string LogoFilePath,
    string LogoSasUrl,
    string LogoFileName,
    string Status);
