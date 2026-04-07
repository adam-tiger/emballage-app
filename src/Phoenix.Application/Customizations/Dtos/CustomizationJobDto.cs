namespace Phoenix.Application.Customizations.Dtos;

/// <summary>
/// DTO complet d'un job de personnalisation, retourné par les queries et handlers
/// de finalisation. La propriété <see cref="LogoSasUrl"/> est enrichie manuellement
/// après le mapping Mapperly — elle n'est jamais persistée en base.
/// </summary>
public sealed record CustomizationJobDto
{
    /// <summary>Identifiant unique du job de personnalisation.</summary>
    public Guid Id { get; init; }

    /// <summary>Identifiant du produit du catalogue associé.</summary>
    public Guid ProductId { get; init; }

    /// <summary>Identifiant de la variante produit sélectionnée.</summary>
    public Guid ProductVariantId { get; init; }

    /// <summary>
    /// Identifiant du client propriétaire du job.
    /// <c>null</c> si le configurateur est utilisé en mode invité.
    /// </summary>
    public Guid? CustomerId { get; init; }

    /// <summary>
    /// Statut courant du job sous forme de chaîne
    /// (ex : <c>"Draft"</c>, <c>"LogoUploaded"</c>, <c>"ReadyForOrder"</c>).
    /// </summary>
    public string Status { get; init; } = string.Empty;

    // ── Logo ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Chemin relatif du logo dans le Blob Storage Azure.
    /// <c>null</c> si aucun logo n'a encore été uploadé.
    /// </summary>
    public string? LogoFilePath { get; init; }

    /// <summary>Nom de fichier original du logo. <c>null</c> avant upload.</summary>
    public string? LogoFileName { get; init; }

    /// <summary>Type MIME du logo (ex : <c>image/svg+xml</c>). <c>null</c> avant upload.</summary>
    public string? LogoContentType { get; init; }

    /// <summary>
    /// URL SAS (Shared Access Signature) temporaire (1h) pour accéder au logo dans Azure Blob.
    /// <br/>
    /// <strong>Non persistée en base</strong> — générée à la demande et enrichie
    /// manuellement après le mapping Mapperly.
    /// La propriété est <c>set</c> (pas <c>init</c>) pour permettre l'enrichissement.
    /// </summary>
    public string? LogoSasUrl { get; set; }

    // ── Position du logo ──────────────────────────────────────────────────────

    /// <summary>Position horizontale du logo en % du canvas (0-100).</summary>
    public decimal PositionX { get; init; }

    /// <summary>Position verticale du logo en % du canvas (0-100).</summary>
    public decimal PositionY { get; init; }

    /// <summary>Facteur d'échelle horizontal du logo (0.1-3.0).</summary>
    public decimal ScaleX { get; init; }

    /// <summary>Facteur d'échelle vertical du logo (0.1-3.0).</summary>
    public decimal ScaleY { get; init; }

    /// <summary>Rotation du logo en degrés (-180 à 180).</summary>
    public decimal Rotation { get; init; }

    // ── Options d'impression ──────────────────────────────────────────────────

    /// <summary>Face(s) d'impression sous forme de chaîne (ex : <c>"SingleSide"</c>, <c>"DoubleSide"</c>).</summary>
    public string PrintSide { get; init; } = "SingleSide";

    /// <summary>Nombre de couleurs sous forme de chaîne (ex : <c>"One"</c>, <c>"FourCMYK"</c>).</summary>
    public string ColorCount { get; init; } = "One";

    /// <summary>
    /// Coefficient global d'impression (PrintSide × ColorCount).
    /// Enrichi manuellement via <c>job.CalculatePrintCoefficient()</c> après le mapping.
    /// </summary>
    public decimal PrintCoefficient { get; init; }

    // ── Audit ─────────────────────────────────────────────────────────────────

    /// <summary>Date et heure de création du job (UTC).</summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>Date et heure de la dernière modification du job (UTC).</summary>
    public DateTime UpdatedAtUtc { get; init; }
}
