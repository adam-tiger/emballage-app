namespace Phoenix.Domain.Customizations.ValueObjects;

// PostgreSQL : stocké en string via HasConversion<string>()

/// <summary>
/// Statut du cycle de vie d'un <see cref="Phoenix.Domain.Customizations.Entities.CustomizationJob"/>.
/// Représente la progression depuis la création jusqu'à la confirmation finale.
/// </summary>
public enum CustomizationStatus
{
    /// <summary>
    /// Job créé, logo pas encore uploadé.
    /// État initial à la création du job via <c>CustomizationJob.Create()</c>.
    /// </summary>
    Draft,

    /// <summary>
    /// Logo uploadé, prêt pour la sélection des options d'impression.
    /// Atteint après un appel réussi à <c>CustomizationJob.UploadLogo()</c>.
    /// </summary>
    LogoUploaded,

    /// <summary>
    /// Options finalisées, prêt pour la création d'une commande ou d'un devis.
    /// Atteint après un appel réussi à <c>CustomizationJob.Finalize()</c>.
    /// </summary>
    ReadyForOrder
}
