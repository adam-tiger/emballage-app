namespace Phoenix.Domain.Customizations.Exceptions;

/// <summary>
/// Exception levée lorsqu'un invariant du domaine <c>Customization</c> est violé.
/// Chaque cas métier est identifié par un code string unique (<see cref="Code"/>).
/// </summary>
public sealed class CustomizationDomainException : Exception
{
    /// <summary>Code métier identifiant la règle de domaine violée (ex : "LOGO_NOT_UPLOADED").</summary>
    public string Code { get; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CustomizationDomainException"/>.
    /// </summary>
    /// <param name="code">Code unique identifiant la règle de domaine violée.</param>
    /// <param name="message">Message lisible décrivant la violation.</param>
    public CustomizationDomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    // ── Codes d'erreur — Job ─────────────────────────────────────────────────

    /// <summary>L'identifiant du produit est obligatoire et ne peut pas être vide.</summary>
    public const string ProductIdRequired = "PRODUCT_ID_REQUIRED";

    /// <summary>L'identifiant de la variante produit est obligatoire et ne peut pas être vide.</summary>
    public const string VariantIdRequired = "VARIANT_ID_REQUIRED";

    /// <summary>
    /// L'opération requiert qu'un logo ait déjà été uploadé
    /// (statut <c>LogoUploaded</c> ou <c>ReadyForOrder</c>).
    /// </summary>
    public const string LogoNotUploaded = "LOGO_NOT_UPLOADED";

    /// <summary>
    /// La finalisation du job est impossible sans logo uploadé au préalable.
    /// Le statut doit être <c>LogoUploaded</c> pour appeler <c>Finalize()</c>.
    /// </summary>
    public const string CannotFinalizeWithoutLogo = "CANNOT_FINALIZE_WITHOUT_LOGO";

    // ── Codes d'erreur — Position logo ───────────────────────────────────────

    /// <summary>
    /// La position du logo est invalide : PositionX ou PositionY hors de [0, 100],
    /// ou rotation hors de [-180, 180].
    /// </summary>
    public const string InvalidLogoPosition = "INVALID_LOGO_POSITION";

    /// <summary>
    /// L'échelle du logo est invalide : ScaleX ou ScaleY hors de [0.1, 3.0].
    /// </summary>
    public const string InvalidScale = "INVALID_SCALE";
}
