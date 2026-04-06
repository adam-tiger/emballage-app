using Phoenix.Domain.Customizations.Events;
using Phoenix.Domain.Customizations.Exceptions;
using Phoenix.Domain.Customizations.ValueObjects;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Domain.Customizations.Entities;

/// <summary>
/// Agrégat racine représentant un job de personnalisation d'emballage.
/// Encapsule le cycle de vie complet d'une personnalisation client :
/// création → upload logo → options impression → finalisation.
/// </summary>
/// <remarks>
/// Un <c>CustomizationJob</c> est distinct d'une commande (<c>Order</c>, Module 5) :
/// il représente l'acte de personnalisation lui-même. Son <c>Id</c> est référencé
/// dans <c>OrderLine.CustomizationJobId</c> lors de la création de la commande.
/// Le configurateur fonctionne en mode invité (<see cref="CustomerId"/> nullable).
/// </remarks>
public sealed class CustomizationJob
{
    // ── Backing fields ───────────────────────────────────────────────────────

    private readonly List<object> _domainEvents = [];

    // ── Propriétés publiques ─────────────────────────────────────────────────

    /// <summary>Identifiant unique du job de personnalisation.</summary>
    public Guid Id { get; private init; }

    /// <summary>Identifiant du produit du catalogue associé à ce job.</summary>
    public Guid ProductId { get; private init; }

    /// <summary>Identifiant de la variante produit sélectionnée (format, taille, matière).</summary>
    public Guid ProductVariantId { get; private init; }

    /// <summary>
    /// Identifiant du client propriétaire du job.
    /// <c>null</c> si le configurateur est utilisé en mode invité (sans compte).
    /// </summary>
    public Guid? CustomerId { get; private init; }

    /// <summary>
    /// Statut courant du cycle de vie du job.
    /// Valeur initiale : <see cref="CustomizationStatus.Draft"/>.
    /// </summary>
    public CustomizationStatus Status { get; private set; } = CustomizationStatus.Draft;

    // ── Logo ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Chemin relatif du logo dans le Blob Storage Azure
    /// (ex : <c>logos/{jobId}/original.svg</c>).
    /// <c>null</c> tant que le logo n'a pas été uploadé.
    /// </summary>
    public string? LogoFilePath { get; private set; }

    /// <summary>
    /// Nom de fichier original du logo uploadé par le client
    /// (ex : <c>mon-logo-restaurant.svg</c>).
    /// <c>null</c> tant que le logo n'a pas été uploadé.
    /// </summary>
    public string? LogoFileName { get; private set; }

    /// <summary>
    /// Type MIME du logo uploadé (ex : <c>image/svg+xml</c>, <c>image/png</c>).
    /// <c>null</c> tant que le logo n'a pas été uploadé.
    /// </summary>
    public string? LogoContentType { get; private set; }

    /// <summary>
    /// URL SAS (Shared Access Signature) temporaire permettant au client d'accéder
    /// directement au logo dans le Blob Storage Azure.
    /// <br/>
    /// <strong>⚠ Non persistée en base de données</strong> — générée à la demande
    /// via <c>IBlobStorageService</c> dans la couche Application.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? LogoSasUrl { get; set; }

    // ── Position du logo ──────────────────────────────────────────────────────

    /// <summary>
    /// Position et transformation du logo sur le canvas 2D du configurateur.
    /// Valeur par défaut : logo centré, taille originale, sans rotation.
    /// </summary>
    public LogoPosition LogoPosition { get; private set; } = LogoPosition.Default;

    // ── Options d'impression ──────────────────────────────────────────────────

    /// <summary>
    /// Face(s) d'impression sélectionnée(s) pour ce job.
    /// Valeur par défaut : <see cref="PrintSide.SingleSide"/> (recto uniquement).
    /// </summary>
    public PrintSide PrintSide { get; private set; } = PrintSide.SingleSide;

    /// <summary>
    /// Nombre de couleurs d'impression sélectionné pour ce job.
    /// Valeur par défaut : <see cref="ColorCount.One"/> (1 couleur Pantone).
    /// </summary>
    public ColorCount ColorCount { get; private set; } = ColorCount.One;

    // ── Audit ─────────────────────────────────────────────────────────────────

    /// <summary>Date et heure de création du job (UTC).</summary>
    public DateTime CreatedAtUtc { get; private init; }

    /// <summary>Date et heure de la dernière modification du job (UTC).</summary>
    public DateTime UpdatedAtUtc { get; private set; }

    // ── Domain events ─────────────────────────────────────────────────────────

    /// <summary>
    /// Événements de domaine en attente de publication.
    /// Vidés après dispatch par le <c>IUnitOfWork</c> lors du <c>SaveChangesAsync</c>.
    /// </summary>
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    // ── Constructeur ─────────────────────────────────────────────────────────

    /// <summary>Constructeur privé — utiliser <see cref="Create"/> pour instancier.</summary>
    private CustomizationJob() { }

    // ── Factory method ────────────────────────────────────────────────────────

    /// <summary>
    /// Crée un nouveau job de personnalisation après validation des invariants de domaine.
    /// Le job est initialisé avec le statut <see cref="CustomizationStatus.Draft"/>.
    /// Émet un <see cref="CustomizationJobCreatedEvent"/>.
    /// </summary>
    /// <param name="productId">Identifiant du produit du catalogue (obligatoire).</param>
    /// <param name="productVariantId">Identifiant de la variante produit (obligatoire).</param>
    /// <param name="customerId">
    /// Identifiant du client propriétaire (nullable — mode invité possible).
    /// </param>
    /// <returns>Une nouvelle instance de <see cref="CustomizationJob"/>.</returns>
    /// <exception cref="CustomizationDomainException">
    /// Levée si <paramref name="productId"/> ou <paramref name="productVariantId"/>
    /// sont égaux à <see cref="Guid.Empty"/>.
    /// </exception>
    public static CustomizationJob Create(
        Guid  productId,
        Guid  productVariantId,
        Guid? customerId)
    {
        if (productId == Guid.Empty)
            throw new CustomizationDomainException(
                CustomizationDomainException.ProductIdRequired,
                "L'identifiant du produit est obligatoire.");

        if (productVariantId == Guid.Empty)
            throw new CustomizationDomainException(
                CustomizationDomainException.VariantIdRequired,
                "L'identifiant de la variante produit est obligatoire.");

        var now = DateTime.UtcNow;

        var job = new CustomizationJob
        {
            Id               = Guid.NewGuid(),
            ProductId        = productId,
            ProductVariantId = productVariantId,
            CustomerId       = customerId,
            Status           = CustomizationStatus.Draft,
            LogoPosition     = LogoPosition.Default,
            PrintSide        = PrintSide.SingleSide,
            ColorCount       = ColorCount.One,
            CreatedAtUtc     = now,
            UpdatedAtUtc     = now
        };

        job._domainEvents.Add(new CustomizationJobCreatedEvent(
            JobId:            job.Id,
            ProductId:        job.ProductId,
            ProductVariantId: job.ProductVariantId,
            CustomerId:       job.CustomerId,
            OccurredAtUtc:    now));

        return job;
    }

    // ── Méthodes publiques ────────────────────────────────────────────────────

    /// <summary>
    /// Enregistre le logo uploadé par le client et passe le job au statut
    /// <see cref="CustomizationStatus.LogoUploaded"/>.
    /// La position du logo est réinitialisée à <see cref="LogoPosition.Default"/> (centré).
    /// Émet un <see cref="LogoUploadedEvent"/>.
    /// </summary>
    /// <param name="filePath">
    /// Chemin relatif du logo dans le Blob Storage Azure (obligatoire).
    /// </param>
    /// <param name="fileName">Nom de fichier original du logo (ex : <c>mon-logo.svg</c>).</param>
    /// <param name="contentType">Type MIME du logo (ex : <c>image/svg+xml</c>).</param>
    /// <exception cref="CustomizationDomainException">
    /// Levée si <paramref name="filePath"/> est vide ou null.
    /// </exception>
    public void UploadLogo(string filePath, string fileName, string contentType)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new CustomizationDomainException(
                CustomizationDomainException.LogoNotUploaded,
                "Le chemin du fichier logo est obligatoire.");

        LogoFilePath    = filePath;
        LogoFileName    = fileName;
        LogoContentType = contentType;
        LogoPosition    = LogoPosition.Default;
        Status          = CustomizationStatus.LogoUploaded;
        UpdatedAtUtc    = DateTime.UtcNow;

        _domainEvents.Add(new LogoUploadedEvent(
            JobId:         Id,
            LogoFilePath:  filePath,
            OccurredAtUtc: UpdatedAtUtc));
    }

    /// <summary>
    /// Met à jour la position et la transformation du logo sur le canvas.
    /// Requiert que le logo ait été préalablement uploadé (statut ≠ Draft).
    /// </summary>
    /// <param name="position">Nouvelle position et transformation du logo.</param>
    /// <exception cref="CustomizationDomainException">
    /// Levée si le statut est <see cref="CustomizationStatus.Draft"/>
    /// (logo pas encore uploadé) ou si la position est invalide.
    /// </exception>
    public void UpdateLogoPosition(LogoPosition position)
    {
        if (Status == CustomizationStatus.Draft)
            throw new CustomizationDomainException(
                CustomizationDomainException.LogoNotUploaded,
                "Impossible de mettre à jour la position du logo : " +
                "aucun logo n'a encore été uploadé pour ce job.");

        LogoPosition = position;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Met à jour les options d'impression (face et nombre de couleurs).
    /// Peut être appelé à tout moment du cycle de vie du job.
    /// </summary>
    /// <param name="printSide">Face(s) à imprimer (recto ou recto-verso).</param>
    /// <param name="colorCount">Nombre de couleurs d'impression (1, 2, 3 ou 4 CMJN).</param>
    public void UpdatePrintOptions(PrintSide printSide, ColorCount colorCount)
    {
        PrintSide    = printSide;
        ColorCount   = colorCount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Calcule le coefficient global d'impression combinant la face d'impression
    /// et le nombre de couleurs.
    /// Logique intentionnellement dupliquée depuis <c>ProductVariant</c> pour respecter
    /// l'indépendance du domaine (pas de couplage entre agrégats).
    /// </summary>
    /// <returns>
    /// Coefficient multiplicateur à appliquer au prix de base de la variante
    /// (ex : recto-verso + 2 couleurs → <c>1.15 × 1.10 = 1.265</c>).
    /// </returns>
    public decimal CalculatePrintCoefficient()
    {
        var printCoeff = PrintSide switch
        {
            PrintSide.SingleSide => 1.00m,
            PrintSide.DoubleSide => 1.15m,
            _                    => 1.00m
        };

        var colorCoeff = ColorCount switch
        {
            ColorCount.One     => 1.00m,
            ColorCount.Two     => 1.10m,
            ColorCount.Three   => 1.18m,
            ColorCount.FourCMYK => 1.25m,
            _                  => 1.00m
        };

        return printCoeff * colorCoeff;
    }

    /// <summary>
    /// Finalise le job de personnalisation et le passe au statut
    /// <see cref="CustomizationStatus.ReadyForOrder"/>.
    /// Le job peut ensuite être référencé dans une <c>OrderLine</c> (Module 5).
    /// Émet un <see cref="CustomizationFinalizedEvent"/>.
    /// </summary>
    /// <exception cref="CustomizationDomainException">
    /// Levée si le statut n'est pas <see cref="CustomizationStatus.LogoUploaded"/>
    /// (logo absent ou job déjà finalisé).
    /// </exception>
    public void Finalize()
    {
        if (Status != CustomizationStatus.LogoUploaded)
            throw new CustomizationDomainException(
                CustomizationDomainException.CannotFinalizeWithoutLogo,
                "La finalisation du job est impossible : " +
                "le logo doit être uploadé avant de finaliser la personnalisation.");

        Status       = CustomizationStatus.ReadyForOrder;
        UpdatedAtUtc = DateTime.UtcNow;

        _domainEvents.Add(new CustomizationFinalizedEvent(
            JobId:            Id,
            CustomerId:       CustomerId,
            ProductVariantId: ProductVariantId,
            PrintSide:        PrintSide,
            ColorCount:       ColorCount,
            OccurredAtUtc:    UpdatedAtUtc));
    }

    /// <summary>
    /// Vide la liste des événements de domaine en attente.
    /// Appelé par <c>IUnitOfWork</c> après le dispatch des événements lors du
    /// <c>SaveChangesAsync</c>.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
