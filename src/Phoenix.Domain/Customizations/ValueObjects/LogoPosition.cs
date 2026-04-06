using Phoenix.Domain.Customizations.Exceptions;

namespace Phoenix.Domain.Customizations.ValueObjects;

/// <summary>
/// Value object immuable représentant la position et la transformation
/// du logo client sur le canvas 2D du configurateur.
/// Toutes les valeurs sont exprimées en pourcentage ou en facteur relatif
/// pour garantir l'indépendance vis-à-vis des dimensions réelles du canvas.
/// </summary>
public sealed record LogoPosition
{
    // ── Constantes de validation ──────────────────────────────────────────────

    private const decimal PositionMin = 0m;
    private const decimal PositionMax = 100m;
    private const decimal ScaleMin    = 0.1m;
    private const decimal ScaleMax    = 3.0m;
    private const decimal RotationMin = -180m;
    private const decimal RotationMax = 180m;

    // ── Propriétés ────────────────────────────────────────────────────────────

    /// <summary>
    /// Position horizontale du centre du logo, exprimée en pourcentage de la largeur
    /// du canvas (0 = bord gauche, 100 = bord droit).
    /// </summary>
    public decimal PositionX { get; init; }

    /// <summary>
    /// Position verticale du centre du logo, exprimée en pourcentage de la hauteur
    /// du canvas (0 = bord supérieur, 100 = bord inférieur).
    /// </summary>
    public decimal PositionY { get; init; }

    /// <summary>
    /// Facteur d'échelle horizontal du logo (plage : 0.1 à 3.0).
    /// <c>1.0</c> correspond à la taille originale du logo.
    /// </summary>
    public decimal ScaleX { get; init; }

    /// <summary>
    /// Facteur d'échelle vertical du logo (plage : 0.1 à 3.0).
    /// <c>1.0</c> correspond à la taille originale du logo.
    /// </summary>
    public decimal ScaleY { get; init; }

    /// <summary>
    /// Angle de rotation du logo en degrés (plage : -180 à 180).
    /// Les valeurs positives effectuent une rotation dans le sens horaire.
    /// </summary>
    public decimal Rotation { get; init; }

    // ── Constructeur ──────────────────────────────────────────────────────────

    /// <summary>
    /// Initialise et valide un <see cref="LogoPosition"/>.
    /// </summary>
    /// <param name="positionX">Position horizontale en % (0-100).</param>
    /// <param name="positionY">Position verticale en % (0-100).</param>
    /// <param name="scaleX">Facteur d'échelle horizontal (0.1 – 3.0).</param>
    /// <param name="scaleY">Facteur d'échelle vertical (0.1 – 3.0).</param>
    /// <param name="rotation">Rotation en degrés (-180 à 180).</param>
    /// <exception cref="CustomizationDomainException">
    /// Levée si l'une des valeurs dépasse les bornes autorisées.
    /// </exception>
    public LogoPosition(
        decimal positionX,
        decimal positionY,
        decimal scaleX,
        decimal scaleY,
        decimal rotation)
    {
        if (positionX < PositionMin || positionX > PositionMax)
            throw new CustomizationDomainException(
                CustomizationDomainException.InvalidLogoPosition,
                $"PositionX doit être comprise entre {PositionMin} et {PositionMax} " +
                $"(valeur reçue : {positionX}).");

        if (positionY < PositionMin || positionY > PositionMax)
            throw new CustomizationDomainException(
                CustomizationDomainException.InvalidLogoPosition,
                $"PositionY doit être comprise entre {PositionMin} et {PositionMax} " +
                $"(valeur reçue : {positionY}).");

        if (scaleX < ScaleMin || scaleX > ScaleMax)
            throw new CustomizationDomainException(
                CustomizationDomainException.InvalidScale,
                $"ScaleX doit être compris entre {ScaleMin} et {ScaleMax} " +
                $"(valeur reçue : {scaleX}).");

        if (scaleY < ScaleMin || scaleY > ScaleMax)
            throw new CustomizationDomainException(
                CustomizationDomainException.InvalidScale,
                $"ScaleY doit être compris entre {ScaleMin} et {ScaleMax} " +
                $"(valeur reçue : {scaleY}).");

        if (rotation < RotationMin || rotation > RotationMax)
            throw new CustomizationDomainException(
                CustomizationDomainException.InvalidLogoPosition,
                $"La rotation doit être comprise entre {RotationMin}° et {RotationMax}° " +
                $"(valeur reçue : {rotation}).");

        PositionX = positionX;
        PositionY = positionY;
        ScaleX    = scaleX;
        ScaleY    = scaleY;
        Rotation  = rotation;
    }

    // ── Valeur par défaut ─────────────────────────────────────────────────────

    /// <summary>
    /// Position par défaut : logo centré sur le canvas, taille originale, sans rotation.
    /// Appliquée automatiquement lors de l'upload initial du logo via
    /// <see cref="Phoenix.Domain.Customizations.Entities.CustomizationJob.UploadLogo"/>.
    /// </summary>
    public static LogoPosition Default => new(50m, 50m, 1.0m, 1.0m, 0m);
}
