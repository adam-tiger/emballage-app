# Prompt Master — Module Configurateur 2D & Personnalisation
# Phoenix Emballages — Sprint 3
# Prêt à coller dans Claude Code (onglet Code, Claude Desktop)

> VERSION  : 1.0 — Stack Angular 21 / .NET 10 / EF Core 10 / PostgreSQL / Konva.js
> MODULE   : Customization (Configurateur 2D)
> SPRINT   : 3 — Configurateur personnalisation produit + upload logo + aperçu temps réel
> DURÉE    : ~22h dev
> PRÉREQUIS: Module 1 (Product & Catalog) DoD validé ✅
>            Module 2 (Auth & Customer) DoD validé ✅
> STATUT   : Prêt à exécuter

---

## ══════════════════════════════════════════
## PROMPT — COLLER DIRECTEMENT DANS CLAUDE CODE
## ══════════════════════════════════════════

Tu es un Lead Full Stack Angular 21 / ASP.NET Core 10 expert en Clean Architecture,
CQRS/MediatR, EF Core 10, PostgreSQL, Azure Blob Storage, SkiaSharp, et **Konva.js**
pour la manipulation de canvas 2D.

Tu génères du code **production-ready**, **complet**, **sans pseudo-code**, sans placeholder.
Chaque fichier est livré en entier. Zéro instruction "à compléter".

---

## CONTEXTE PROJET

- **Produit**    : Phoenix Emballages — E-commerce B2B d'emballages alimentaires personnalisables
- **Type**       : B2B + B2C — site public + back-office admin + espace client
- **Stack back** : .NET 10, ASP.NET Core 10, EF Core 10, **PostgreSQL** (Npgsql), MediatR,
                   FluentValidation, AutoMapper, Azure Blob Storage, SkiaSharp, QuestPDF, Serilog
- **Stack front**: Angular 21 standalone SSR, PrimeNG 18, Angular Signals, Reactive Forms,
                   **Konva.js** (configurateur 2D canvas)
- **Hébergement**: Azure App Service Linux + Azure Database for PostgreSQL Flexible Server
                   + Azure Blob Storage + Azure CDN
- **Auth**       : ASP.NET Identity + JWT Bearer (access token) + Cookie HttpOnly (refresh token)
                   → Module 2 déjà implémenté
- **Langue code**: EN — Labels UI : FR

### Modules déjà implémentés (ne PAS toucher)

```
Module 1 — Product & Catalog
  → Product, ProductVariant, PriceTier, ProductImage, ProductFamily
  → IBlobStorageService, IImageProcessingService (SkiaSharp WebP)
  → AzureBlobStorageService, ImageProcessingService
  → CRUD produits admin + catalogue public
  → Seed 10 produits réels du catalogue

Module 2 — Auth & Customer
  → ApplicationUser, Customer, CustomerAddress
  → JWT access 15 min + refresh Cookie HttpOnly 7j
  → AuthService Angular (signals), jwt.interceptor, guards
  → Login, Register, espace client (dashboard, profil, adresses)
```

---

## STANDARDS OBLIGATOIRES

Respecter strictement tous ces fichiers du projet :

- `02-architecture.md`         → Clean Archi, couches, Blob Storage, patterns
- `03-naming-conventions.md`   → PascalCase, camelCase, kebab-case, ubiquitous language
- `04-backend-standards-dotnet.md` → Handlers MediatR, FluentValidation, contrat erreur uniforme
- `05-frontend-standards-angular.md` → Standalone, Signals, OnPush, loading/error/empty
- `06-api-contract-standards.md` → REST /api/v1/, pagination, codes HTTP
- `07-database.md`             → IEntityTypeConfiguration, index, soft delete, audit
- `09-definition-of-done.md`   → Tests, Swagger, auth, logs, checklist finale
- `PHOENIX-domain.md`          → Ubiquitous language, CustomizationJob, coefficients impression

---

## OBJECTIF DU MODULE

Générer le module **Customization (Configurateur 2D)** complet.
C'est le **DIFFÉRENCIATEUR MÉTIER** de Phoenix Emballages — la personnalisation
d'emballages avec logo client est la source principale de valeur (70% du CA).

Le client sélectionne un produit personnalisable, choisit ses options
(face d'impression, nombre de couleurs, quantité), uploade son logo,
le positionne visuellement sur le canvas 2D, voit le prix en temps réel,
puis confirme sa personnalisation.

### Périmètre fonctionnel

**Configurateur 2D (authentifié, rôle Customer) :**
- Sélection variante produit (ProductVariant) + quantité
- Choix PrintSide (SingleSide / DoubleSide) et ColorCount (1 / 2 / 3 / 4 CMJN)
- Upload logo client (SVG, PDF vectoriel, PNG ≥300 DPI, AI — max 20 MB)
- Canvas Konva.js : aperçu produit + logo positionnable (drag, resize, rotate)
- Centrage automatique du logo au placement initial
- Vue recto / vue recto-verso (si DoubleSide sélectionné)
- Calcul de prix en temps réel affiché pendant la configuration
- Confirmation de la personnalisation → CustomizationJob confirmé
- Le JobId sera référencé dans OrderLine au Module 5

**Espace client — Mes personnalisations :**
- Liste des jobs du client (avec statut, produit, date)
- Détail d'un job avec aperçu du rendu

---

## DÉCISIONS TECHNIQUES VALIDÉES — Configurateur

```
1. CANVAS : KONVA.JS VIA @defer
   Konva.js est LOURD (~300 KB) et incompatible SSR (dépend de window/canvas)
   → Chargé via @defer dans Angular 21 (lazy loading SSR-compatible)
   → JAMAIS dans le bundle principal
   → Encapsuler dans un composant CanvasPreviewComponent chargé à la demande

2. STOCKAGE CUSTOMIZATIONJOB EN POSTGRESQL
   Table CustomizationJob + table LogoFile via EF Core 10
   Pas de stockage NoSQL — PostgreSQL suffit pour ce volume

3. LOGO UPLOAD : AZURE BLOB STORAGE
   Container : phoenix-customer-logos (PRIVATE)
   Accès via SAS token (durée 1h) — jamais d'URL publique
   IBlobStorageService existant du Module 1 → réutiliser tel quel
   Blob path : customers/{customerId}/logos/{jobId}/{filename}.png

4. APERÇU WEBP : SKIA SHARP
   IImageProcessingService existant du Module 1 → réutiliser
   Logo uploadé : converti en PNG 300 DPI via SkiaSharp avant stockage
   Aperçu canvas : généré côté .NET si nécessaire (pour BAT futur)

5. POSITION LOGO EN POURCENTAGE (0-100)
   LogoPositionX, LogoPositionY → % du canvas (0.0 à 100.0)
   LogoScaleX, LogoScaleY → facteur d'échelle (ex: 0.5 à 2.0)
   LogoRotation → degrés (0 à 360)
   Avantage : indépendant de la résolution d'affichage
   Le canvas Konva.js convertit pixels ↔ pourcentages en temps réel

6. CALCUL DE PRIX TEMPS RÉEL CÔTÉ ANGULAR
   Formule identique au Domain :
     UnitPriceHT = BasePriceTier.UnitPriceHT × PrintSideCoeff × ColorCountCoeff
     TotalHT     = UnitPriceHT × Quantity
     TVA         = TotalHT × 0.20
     TotalTTC    = TotalHT + TVA
   Les coefficients PrintSide et ColorCount du Domain Module 1 sont réutilisés
   Le backend valide le prix final au moment du Confirm

7. STATE MACHINE STRICTE — JobStatus
   Draft → LogoUploaded → Confirmed     (flux nominal)
   Draft → Cancelled                     (uniquement depuis Draft)
   Confirmed → ne peut PAS revenir en arrière
   LogoUploaded → ne peut PAS revenir à Draft
   Toute transition invalide → DomainException

8. LIEN VERS ORDER (Module 5 futur)
   CustomizationJob.Id sera référencé dans OrderLine.CustomizationJobId
   → Ne PAS créer OrderLine maintenant, juste prévoir la FK Guid? nullable
```

---

## SPÉCIFICATIONS MÉTIER IMPÉRATIVES (PHOENIX-domain.md)

### Règles métier Personnalisation

```
1. PRODUIT PERSONNALISABLE UNIQUEMENT
   Seuls les produits avec Product.IsCustomizable = true peuvent entrer
   dans le configurateur
   Validation serveur : si Product.IsCustomizable == false → 400 Bad Request

2. MOQ VALIDATION
   CustomizationJob.Quantity >= ProductVariant.MinimumOrderQuantity
   Validation côté Angular (temps réel) + côté serveur (FluentValidation)

3. FORMATS LOGO ACCEPTÉS
   SVG (recommandé), PDF vectoriel, PNG ≥300 DPI, AI
   Taille maximale : 20 MB
   Validation côté Angular (extension + taille) + côté serveur (content type + taille)
   Extensions autorisées : .svg, .pdf, .png, .ai

4. COEFFICIENTS D'IMPRESSION
   PrintSide.SingleSide  → ×1.00 (base)
   PrintSide.DoubleSide  → ×1.15 (+15% — exclusif Phoenix)
   ColorCount.One        → ×1.00
   ColorCount.Two        → ×1.10
   ColorCount.Three      → ×1.18
   ColorCount.FourCMYK   → ×1.25

5. RECTO-VERSO (DoubleSide)
   Disponible sur TOUS les produits de la Gamme Gourmet
   C'est le différenciateur clé de Phoenix
   Si DoubleSide → afficher les 2 faces dans le canvas (toggle vue)

6. BAT (BON À TIRER) — MODULE FUTUR
   Après confirmation job, le BAT sera généré au Module 4/5
   Pour ce module : on confirme le job, le BAT viendra plus tard
   Prévoir un champ BatStatus? nullable sur CustomizationJob (non utilisé encore)
```

### Enums domaine

```csharp
// JobStatus (nouveau — ce module)
Draft, LogoUploaded, Confirmed, Cancelled

// PrintSide (existant Module 1)
SingleSide, DoubleSide

// ColorCount (existant Module 1)
One, Two, Three, FourCMYK

// CustomerSegment (existant Module 2 — pour info contexte)
FastFood, BakeryPastry, JapaneseAsian, BubbleTea, RetailCommerce,
FoodTruck, Catering, ChocolateConfectionery, PizzaShop, Other
```

### Permissions endpoints

```
POST /api/v1/customizations                    → [Authorize(Roles = "Customer")]
PUT  /api/v1/customizations/{id}/logo          → [Authorize(Roles = "Customer")]
PUT  /api/v1/customizations/{id}/position      → [Authorize(Roles = "Customer")]
POST /api/v1/customizations/{id}/confirm       → [Authorize(Roles = "Customer")]
GET  /api/v1/customizations/{id}               → [Authorize(Roles = "Customer")]
GET  /api/v1/customizations/my-jobs            → [Authorize(Roles = "Customer")]
DELETE /api/v1/customizations/{id}             → [Authorize(Roles = "Customer")]
  (cancel — uniquement si status = Draft)
```

---

## LIVRABLES ATTENDUS — OBLIGATOIRES

### 1. Arborescence complète

Donner l'arborescence complète backend + frontend AVANT tout code.
Marquer clairement ce qui est AJOUTÉ par rapport aux Modules 1 et 2.

### 2. Domain Layer — code complet

```
Phoenix.Domain/Customizations/
  Entities/
    CustomizationJob.cs            (agrégat racine)
      → Id (Guid)
      → CustomerId (Guid — FK vers Customer)
      → ProductId (Guid — FK vers Product)
      → ProductVariantId (Guid — FK vers ProductVariant)
      → Quantity (int)
      → PrintSide (enum PrintSide)
      → ColorCount (enum ColorCount)
      → LogoPositionX (decimal — % 0-100)
      → LogoPositionY (decimal — % 0-100)
      → LogoScaleX (decimal — facteur ex: 1.0)
      → LogoScaleY (decimal — facteur ex: 1.0)
      → LogoRotation (decimal — degrés 0-360)
      → UnitPriceHT (decimal — calculé)
      → TotalPriceHT (decimal — calculé)
      → Status (JobStatus enum)
      → BatStatus (string? — nullable, futur)
      → CreatedAtUtc, UpdatedAtUtc
      → Logo (LogoFile? — navigation)

      Méthodes métier (encapsulation état) :
        Create(customerId, productId, variantId, quantity, printSide,
               colorCount, unitPriceHT)
          → Valide quantity > 0, unitPriceHT > 0
          → Status = Draft
        UpdateLogo(logoFile)
          → Status doit être Draft ou LogoUploaded
          → Status → LogoUploaded
        UpdatePosition(posX, posY, scaleX, scaleY, rotation)
          → Status doit être LogoUploaded
          → Valide 0 <= posX/posY <= 100
        Confirm()
          → Status doit être LogoUploaded
          → Status → Confirmed
          → Raise CustomizationJobConfirmedEvent
        Cancel()
          → Status doit être Draft
          → Status → Cancelled

    LogoFile.cs                    (entité)
      → Id (Guid)
      → CustomizationJobId (Guid — FK)
      → OriginalFileName (string)
      → BlobPath (string — chemin relatif Blob Storage)
      → ContentType (string)
      → FileSizeBytes (long)
      → WidthPx (int?)
      → HeightPx (int?)
      → UploadedAtUtc (DateTime)

  Enums/
    JobStatus.cs                   (Draft, LogoUploaded, Confirmed, Cancelled)

  Events/
    CustomizationJobCreatedEvent.cs    (JobId, CustomerId, ProductId)
    CustomizationJobConfirmedEvent.cs  (JobId, CustomerId, TotalPriceHT)
    CustomizationJobCancelledEvent.cs  (JobId)

  Repositories/
    ICustomizationJobRepository.cs
      → GetByIdAsync(Guid id)
      → GetByIdWithLogoAsync(Guid id)
      → GetByCustomerIdAsync(Guid customerId, CancellationToken)
      → AddAsync(CustomizationJob job)
      → UpdateAsync(CustomizationJob job)
```

### 3. Application Layer — code complet

```
Phoenix.Application/Customizations/
  Commands/
    CreateJob/
      CreateJobCommand.cs          (ProductId, VariantId, PrintSide,
                                    ColorCount, Quantity)
        → CustomerId extrait des claims JWT (pas en paramètre)
      CreateJobCommandHandler.cs
        → Charge ProductVariant + PriceTier
        → Valide quantity >= MOQ
        → Valide Product.IsCustomizable == true
        → Calcule UnitPriceHT (palier × coeff impression)
        → Crée CustomizationJob via factory method Create()
        → Retourne CreateJobResponse (jobId, calculatedPrice)
      CreateJobCommandValidator.cs
        → ProductId requis, VariantId requis
        → Quantity > 0
        → PrintSide et ColorCount valides

    UpdateJobLogo/
      UpdateJobLogoCommand.cs      (JobId, FileStream, FileName, ContentType)
      UpdateJobLogoCommandHandler.cs
        → Valide le job appartient au customer (claims)
        → Valide format (extension + content type)
        → Valide taille <= 20 MB
        → Upload vers Azure Blob via IBlobStorageService
          Path : customers/{customerId}/logos/{jobId}/{filename}.png
        → Si PNG : redimensionner/optimiser via IImageProcessingService (300 DPI)
        → Crée LogoFile, appelle job.UpdateLogo(logoFile)
        → Si logo précédent existe → supprimer ancien blob
      UpdateJobLogoCommandValidator.cs

    UpdateJobPosition/
      UpdateJobPositionCommand.cs  (JobId, LogoPositionX, LogoPositionY,
                                    LogoScaleX, LogoScaleY, LogoRotation)
      UpdateJobPositionCommandHandler.cs
        → Valide le job appartient au customer
        → Appelle job.UpdatePosition(...)
      UpdateJobPositionCommandValidator.cs
        → PositionX/Y : 0-100
        → ScaleX/Y : 0.1 à 5.0
        → Rotation : 0-360

    ConfirmJob/
      ConfirmJobCommand.cs         (JobId)
      ConfirmJobCommandHandler.cs
        → Valide le job appartient au customer
        → Recalcule le prix final côté serveur (source de vérité)
        → Appelle job.Confirm()
        → Publie CustomizationJobConfirmedEvent
      ConfirmJobCommandValidator.cs

    CancelJob/
      CancelJobCommand.cs          (JobId)
      CancelJobCommandHandler.cs
        → Valide le job appartient au customer
        → Appelle job.Cancel()
        → Supprime le blob logo si existant
      CancelJobCommandValidator.cs

  Queries/
    GetJobById/
      GetJobByIdQuery.cs           (JobId)
      GetJobByIdQueryHandler.cs
        → Valide le job appartient au customer
        → Retourne CustomizationJobDto avec LogoFile + SAS URL
      GetJobByIdQueryValidator.cs

    GetJobsByCustomer/
      GetJobsByCustomerQuery.cs    (page, pageSize, statusFilter?)
      GetJobsByCustomerQueryHandler.cs
        → CustomerId extrait des claims
        → Retourne PaginatedList<CustomizationJobSummaryDto>

  Dtos/
    CustomizationJobDto.cs         (tous les champs + LogoFileDto + ProductInfo)
      → Id, ProductName, VariantName, Quantity, PrintSide, ColorCount
      → LogoPositionX/Y, LogoScaleX/Y, LogoRotation
      → UnitPriceHT, TotalPriceHT, Status
      → Logo: LogoFileDto?
      → CreatedAtUtc, UpdatedAtUtc
    CustomizationJobSummaryDto.cs  (pour la liste)
      → Id, ProductName, Quantity, Status, TotalPriceHT, CreatedAtUtc
    LogoFileDto.cs
      → Id, OriginalFileName, ContentType, FileSizeBytes, LogoUrl (SAS token URL)
    CreateJobResponse.cs
      → JobId, UnitPriceHT, TotalPriceHT, Status
    PriceCalculationDto.cs
      → BasePriceHT, PrintSideCoeff, ColorCountCoeff, UnitPriceHT, TotalHT, TVA, TotalTTC

  Services/
    IPriceCalculationService.cs    (interface Application)
      → CalculateCustomizationPrice(ProductVariant, int quantity,
            PrintSide, ColorCount) : PriceCalculationDto

  Mappings/
    CustomizationMappingProfile.cs
```

### 4. Infrastructure Layer — code complet

```
Phoenix.Infrastructure/
  Persistence/
    Configurations/
      CustomizationJobConfiguration.cs    (IEntityTypeConfiguration<CustomizationJob>)
        → Property configs : precision decimal(18,4) pour les prix
        → HasConversion<string>() pour JobStatus, PrintSide, ColorCount
        → Index sur CustomerId, Status
        → HasOne(j => j.Logo).WithOne().HasForeignKey<LogoFile>(l => l.CustomizationJobId)
        → Relation vers Product (navigation optionnelle pour lecture)
        → Relation vers ProductVariant (navigation optionnelle)
      LogoFileConfiguration.cs            (IEntityTypeConfiguration<LogoFile>)
        → BlobPath max 500 chars
        → Index sur CustomizationJobId

    PhoenixDbContext.cs                   (AJOUTER — ne pas casser Modules 1-2)
      → DbSet<CustomizationJob>
      → DbSet<LogoFile>

    Migrations/
      AddCustomizationModule              (EF Core 10 migration)

  Repositories/
    CustomizationJobRepository.cs         (ICustomizationJobRepository)
      → Include(j => j.Logo) pour GetByIdWithLogoAsync
      → Where(j => j.CustomerId == customerId) pour GetByCustomerIdAsync

  Services/
    PriceCalculationService.cs            (IPriceCalculationService)
      → Charge PriceTier du ProductVariant
      → Applique coefficients PrintSide × ColorCount
      → Calcule UnitPriceHT, TotalHT, TVA (20%), TotalTTC
      → Source de vérité serveur (Angular fait le même calcul pour UX temps réel)
```

### 5. API Layer — code complet

```
Phoenix.Api/Controllers/v1/
  CustomizationsController.cs
    [ApiController]
    [Route("api/v1/customizations")]
    [Authorize(Roles = "Customer")]

    POST /api/v1/customizations                     → CreateJobCommand
      → Body : { productId, variantId, printSide, colorCount, quantity }
      → Retourne 201 Created + CreateJobResponse
      → Location header : /api/v1/customizations/{jobId}

    PUT  /api/v1/customizations/{id}/logo           → UpdateJobLogoCommand
      → Multipart form-data : file (logo)
      → Retourne 200 OK + LogoFileDto

    PUT  /api/v1/customizations/{id}/position       → UpdateJobPositionCommand
      → Body : { logoPositionX, logoPositionY, logoScaleX, logoScaleY, logoRotation }
      → Retourne 200 OK

    POST /api/v1/customizations/{id}/confirm        → ConfirmJobCommand
      → Retourne 200 OK + CustomizationJobDto (avec prix final recalculé)

    DELETE /api/v1/customizations/{id}              → CancelJobCommand
      → Retourne 204 No Content (uniquement si status == Draft)

    GET  /api/v1/customizations/{id}                → GetJobByIdQuery
      → Retourne 200 OK + CustomizationJobDto (avec SAS URL logo)

    GET  /api/v1/customizations/my-jobs             → GetJobsByCustomerQuery
      → Query params : page, pageSize, status (optionnel)
      → Retourne 200 OK + PaginatedList<CustomizationJobSummaryDto>
```

### 6. Frontend Angular 21 — code complet

```
src/app/features/customization/
  pages/
    configurator.page.ts                 (smart — page principale du configurateur)
      → Route : /customization/configure/:productId
      → Layout responsive : canvas à gauche (60%), panneaux à droite (40%)
      → Mobile : panneaux empilés sous le canvas
      → Charge le Product + Variants depuis le ProductCatalogService (Module 1)
      → Orchestration globale : state machine via ConfiguratorService

  components/
    canvas-preview/
      canvas-preview.component.ts        (Konva.js — CHARGÉ VIA @defer)
        → ⚠️ CRITIQUE : ce composant est wrappé dans @defer dans le template parent
        → Initialise Konva.Stage + Konva.Layer
        → Affiche image de fond du produit (photo produit depuis CDN)
        → Affiche le logo client (Konva.Image — drag, resize, rotate)
        → Poignées de transformation Konva.Transformer
        → Centrage automatique du logo au premier placement
        → Émet la position en % à chaque déplacement (via output())
          → Conversion pixels → % : posX% = (node.x() / stage.width()) * 100
        → Responsive : adapte la taille du Stage au container parent
          → ResizeObserver sur le container
          → stage.width(containerWidth), stage.height(containerWidth * aspectRatio)
        → Vue toggle recto/verso si PrintSide === DoubleSide
          → 2 layers : frontLayer + backLayer, toggle visibilité

    variant-selector-panel/
      variant-selector-panel.component.ts  (dumb)
        → input() : variants (ProductVariant[]), selectedVariant
        → output() : variantSelected (ProductVariant)
        → Dropdown PrimeNG p-dropdown pour sélection variante
        → Affiche MOQ de la variante sélectionnée

    print-options-panel/
      print-options-panel.component.ts     (dumb)
        → input() : selectedPrintSide, selectedColorCount
        → output() : printSideChanged, colorCountChanged
        → RadioButtons PrimeNG pour PrintSide : Recto / Recto-Verso
        → Dropdown/RadioButtons PrimeNG pour ColorCount : 1, 2, 3, 4 couleurs
        → Affiche le coefficient appliqué à côté de chaque option
          → ex: "Recto-verso (+15%)"
          → ex: "4 couleurs CMJN (+25%)"

    logo-upload-panel/
      logo-upload-panel.component.ts       (dumb)
        → input() : currentLogo (LogoFileDto?)
        → output() : logoUploaded (File)
        → PrimeNG p-fileUpload
        → Validation côté client :
          - Extensions : .svg, .pdf, .png, .ai
          - Taille max : 20 MB
        → Aperçu du logo uploadé (miniature)
        → Bouton "Remplacer le logo" si déjà uploadé
        → Labels FR : "Téléchargez votre logo", "Formats acceptés : SVG, PDF, PNG, AI"

    quantity-input/
      quantity-input.component.ts          (dumb)
        → input() : minQuantity (MOQ), currentQuantity, soldByWeight
        → output() : quantityChanged (number)
        → PrimeNG p-inputNumber
        → Minimum = MOQ du variant sélectionné
        → Affiche "pièces" ou "KG" selon soldByWeight
        → Erreur inline si quantity < MOQ

    price-summary-panel/
      price-summary-panel.component.ts     (dumb)
        → input() : priceCalculation (PriceCalculation)
        → Affiche en temps réel :
          - Prix unitaire HT (avec coefficients détaillés)
          - Quantité × Prix unitaire
          - Sous-total HT
          - TVA (20%)
          - Total TTC
        → Mise en valeur du prix total (couleur Phoenix #E8552A)
        → Labels FR : "Prix unitaire HT", "Sous-total HT", "TVA (20%)", "Total TTC"

    confirm-button/
      confirm-button.component.ts          (dumb)
        → input() : canConfirm (boolean), isLoading
        → output() : confirmed
        → Bouton PrimeNG p-button "Valider ma personnalisation"
        → Disabled tant que logo non uploadé ou quantity < MOQ
        → Loading state pendant la confirmation

    job-status-badge/
      job-status-badge.component.ts        (dumb)
        → input() : status (JobStatus)
        → PrimeNG p-tag avec couleur selon statut :
          Draft → gris, LogoUploaded → bleu, Confirmed → vert, Cancelled → rouge

  services/
    configurator.service.ts                (state machine Signals)
      → State signals :
        selectedProduct = signal<ProductDetail | null>(null)
        selectedVariant = signal<ProductVariant | null>(null)
        selectedPrintSide = signal<PrintSide>('SingleSide')
        selectedColorCount = signal<ColorCount>('One')
        quantity = signal<number>(0)
        currentJob = signal<CustomizationJob | null>(null)
        logoFile = signal<LogoFile | null>(null)
        logoPosition = signal<LogoPosition>({ x: 50, y: 50, scaleX: 1, scaleY: 1, rotation: 0 })
        isLoading = signal(false)
        error = signal<string | null>(null)

      → Computed signals :
        printSideCoeff = computed(() => PrintSideCoefficients[this.selectedPrintSide()])
        colorCountCoeff = computed(() => ColorCountCoefficients[this.selectedColorCount()])
        basePriceHT = computed(() => {
          // Trouver le PriceTier correspondant à la quantité
          const variant = this.selectedVariant();
          const qty = this.quantity();
          if (!variant || qty <= 0) return 0;
          const tier = variant.priceTiers
            .filter(t => qty >= t.minQuantity)
            .sort((a, b) => b.minQuantity - a.minQuantity)[0];
          return tier?.unitPriceHT ?? 0;
        })
        unitPriceHT = computed(() =>
          this.basePriceHT() * this.printSideCoeff() * this.colorCountCoeff()
        )
        totalHT = computed(() => this.unitPriceHT() * this.quantity())
        tva = computed(() => this.totalHT() * 0.20)
        totalTTC = computed(() => this.totalHT() + this.tva())
        isQuantityValid = computed(() =>
          this.quantity() >= (this.selectedVariant()?.minimumOrderQuantity ?? 0)
        )
        canConfirm = computed(() =>
          this.logoFile() !== null && this.isQuantityValid() && this.currentJob() !== null
        )

      → Méthodes :
        initConfigurator(productId: string): void
          → Charge product detail via ProductCatalogService
          → Pré-sélectionne première variante et MOQ
        selectVariant(variant: ProductVariant): void
          → Met à jour selectedVariant, reset quantity au MOQ
        createJob(): Observable<CreateJobResponse>
          → POST /api/v1/customizations
        uploadLogo(file: File): Observable<LogoFileDto>
          → PUT /api/v1/customizations/{jobId}/logo (multipart)
        updatePosition(position: LogoPosition): void
          → PUT /api/v1/customizations/{jobId}/position
          → Debounce 300ms (pas d'appel API à chaque pixel)
        confirmJob(): Observable<CustomizationJobDto>
          → POST /api/v1/customizations/{jobId}/confirm
        cancelJob(): Observable<void>
          → DELETE /api/v1/customizations/{jobId}
        loadMyJobs(page?, pageSize?, status?): Observable<PaginatedList<...>>
          → GET /api/v1/customizations/my-jobs

    customization-api.service.ts           (HTTP layer pur)
      → Tous les appels HTTP bruts vers /api/v1/customizations
      → withCredentials: true
      → Séparé du ConfiguratorService pour testabilité

  models/
    customization.model.ts
      → CustomizationJob, CustomizationJobSummary, LogoFile, LogoPosition
      → CreateJobRequest, CreateJobResponse, PriceCalculation
      → JobStatus enum type ('Draft' | 'LogoUploaded' | 'Confirmed' | 'Cancelled')
      → PrintSideCoefficients map, ColorCountCoefficients map

  customization.routes.ts
    → /customization/configure/:productId  → ConfiguratorPage
        [AuthGuard + RoleGuard("Customer")]

src/app/features/customer/
  pages/
    customer-jobs.page.ts                  (AJOUTER — liste des personnalisations)
      → PrimeNG p-table paginée avec filtres par statut
      → Colonnes : Produit, Quantité, Statut (badge), Prix TTC, Date, Actions
      → Action "Voir" → dialog détail ou navigate vers job detail
      → Loading / error / empty states
      → Labels FR : "Mes personnalisations", "Aucune personnalisation"

  customer.routes.ts                       (MODIFIER — ajouter route)
    → /customer/jobs → CustomerJobsPage [AuthGuard + RoleGuard("Customer")]
```

#### Angular 21 — contraintes obligatoires (rappel)

```typescript
// Tous les composants : standalone: true, changeDetection: OnPush
// Inputs : input() à la place de @Input()
// Outputs : output() à la place de @Output()
// Services : signal() / computed() pour le state (plus de BehaviorSubject)
// inject() à la place du constructeur
// trackBy sur tout ngFor
// loading / error / empty states sur toutes les listes
// withCredentials: true sur tous les appels API auth

// ⚠️ CRITIQUE — Konva.js dans @defer :
@Component({
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="configurator-layout">
      <div class="canvas-container" #canvasContainer>
        @defer {
          <app-canvas-preview
            [productImageUrl]="productImageUrl()"
            [logoUrl]="logoUrl()"
            [position]="logoPosition()"
            (positionChanged)="onPositionChanged($event)"
          />
        } @loading {
          <div class="canvas-loading">
            <p-progressSpinner />
            <span>Chargement du configurateur...</span>
          </div>
        } @placeholder {
          <div class="canvas-placeholder">
            <span>Sélectionnez un produit pour configurer</span>
          </div>
        }
      </div>
      <div class="panels-container">
        <!-- panneaux de configuration -->
      </div>
    </div>
  `
})
export class ConfiguratorPage { ... }

// ⚠️ Canvas responsive via ResizeObserver :
export class CanvasPreviewComponent implements AfterViewInit, OnDestroy {
  private readonly elementRef = inject(ElementRef);
  private resizeObserver!: ResizeObserver;
  private stage!: Konva.Stage;

  ngAfterViewInit(): void {
    const container = this.elementRef.nativeElement.querySelector('.konva-container');
    this.stage = new Konva.Stage({
      container,
      width: container.offsetWidth,
      height: container.offsetWidth * 0.75 // ratio 4:3 par défaut
    });

    this.resizeObserver = new ResizeObserver(entries => {
      const { width } = entries[0].contentRect;
      this.stage.width(width);
      this.stage.height(width * 0.75);
      this.stage.batchDraw();
    });
    this.resizeObserver.observe(container);
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
    this.stage?.destroy();
  }
}

// ⚠️ Conversion pixels ↔ pourcentages :
private pixelsToPercent(x: number, y: number): { x: number; y: number } {
  return {
    x: (x / this.stage.width()) * 100,
    y: (y / this.stage.height()) * 100
  };
}
private percentToPixels(xPct: number, yPct: number): { x: number; y: number } {
  return {
    x: (xPct / 100) * this.stage.width(),
    y: (yPct / 100) * this.stage.height()
  };
}

// ⚠️ Coefficients côté Angular (miroir du Domain) :
export const PrintSideCoefficients: Record<PrintSide, number> = {
  SingleSide: 1.00,
  DoubleSide: 1.15
};
export const ColorCountCoefficients: Record<ColorCount, number> = {
  One: 1.00,
  Two: 1.10,
  Three: 1.18,
  FourCMYK: 1.25
};
```

### 7. Navigation — liens vers le configurateur

```typescript
// Dans product-detail.page.ts (Module 1) — AJOUTER un bouton :
// Si product.isCustomizable → afficher bouton "Personnaliser ce produit"
// Lien : /customization/configure/{productId}
// Ce bouton n'apparaît que si l'utilisateur est connecté (isAuthenticated)
// Si non connecté → redirect /auth/login?returnUrl=/customization/configure/{productId}

// Dans customer-dashboard.page.ts (Module 2) — AJOUTER une carte :
// Carte "Mes personnalisations" avec lien vers /customer/jobs
```

### 8. Tests

```
Phoenix.UnitTests/Customizations/
  CustomizationJobTests.cs                         (invariants domaine)
    → Create() : status = Draft, prix positif
    → Create() : quantity <= 0 → DomainException
    → UpdateLogo() : Draft → LogoUploaded OK
    → UpdateLogo() : Confirmed → DomainException (state machine)
    → UpdatePosition() : valide % 0-100
    → UpdatePosition() : > 100 → DomainException
    → Confirm() : LogoUploaded → Confirmed OK
    → Confirm() : Draft (sans logo) → DomainException
    → Cancel() : Draft → Cancelled OK
    → Cancel() : Confirmed → DomainException
  CreateJobCommandHandlerTests.cs
    → Produit IsCustomizable=true → crée job OK
    → Produit IsCustomizable=false → erreur 400
    → Quantity < MOQ → erreur validation
    → Calcul prix : palier × printSideCoeff × colorCountCoeff correct
  UpdateJobLogoCommandHandlerTests.cs
    → Upload logo → IBlobStorageService appelé avec bon path
    → Format non autorisé → erreur 400
    → Taille > 20 MB → erreur 400
    → Job appartient à un autre customer → erreur 403
  ConfirmJobCommandHandlerTests.cs
    → Job LogoUploaded → Confirmed + prix recalculé
    → Job Draft (sans logo) → erreur 400
    → Job Cancelled → erreur 400
  PriceCalculationServiceTests.cs
    → Palier correct sélectionné pour quantité donnée
    → Coefficients SingleSide × One = ×1.00
    → Coefficients DoubleSide × FourCMYK = ×1.15 × 1.25 = ×1.4375

Phoenix.IntegrationTests/Customizations/
  CustomizationsControllerTests.cs         (Testcontainers.PostgreSQL)
    → POST /api/v1/customizations → 201 + CreateJobResponse (customer auth)
    → POST /api/v1/customizations → 401 (pas de token)
    → POST /api/v1/customizations → 403 (rôle Employee, pas Customer)
    → PUT  /api/v1/customizations/{id}/logo → 200 (multipart upload)
    → PUT  /api/v1/customizations/{id}/position → 200
    → POST /api/v1/customizations/{id}/confirm → 200 + prix final
    → DELETE /api/v1/customizations/{id} → 204 (uniquement si Draft)
    → DELETE /api/v1/customizations/{id} → 400 (si Confirmed)
    → GET  /api/v1/customizations/my-jobs → 200 + PaginatedList
    → GET  /api/v1/customizations/{id} → 200 + CustomizationJobDto + SAS URL logo
    → GET  /api/v1/customizations/{id} d'un autre customer → 403
```

### 9. Commandes d'exécution

```bash
# ── Backend ────────────────────────────────────────────────
dotnet restore
dotnet build --configuration Release -warnaserror

# Migration PostgreSQL (EF Core 10 + Npgsql)
dotnet ef migrations add AddCustomizationModule \
  --project Phoenix.Infrastructure \
  --startup-project Phoenix.Api

dotnet ef database update \
  --project Phoenix.Infrastructure \
  --startup-project Phoenix.Api

# Tests
dotnet test --collect:"XPlat Code Coverage"

# Run local
dotnet run --project Phoenix.Api

# ── Frontend ───────────────────────────────────────────────
npm ci
# Vérifier que konva est dans package.json :
npm install konva --save
ng serve --open
ng build --configuration production
npm run lint
npm run test -- --watch=false
```

---

## CONTRAINTES DE GÉNÉRATION

```
✅ Code complet — AUCUN pseudo-code, AUCUN placeholder, AUCUN "// TODO"
✅ MVP strict — pas de sur-ingénierie
✅ Hypothèses explicites AVANT le code si nécessaire
✅ Justification 1 ligne max pour les choix structurants
✅ Ne PAS casser les Modules 1 et 2 — ajouter, ne pas modifier
   (sauf PhoenixDbContext pour les nouveaux DbSet + bouton fiche produit)

Backend :
✅ Npgsql — PAS de SqlServer provider
✅ Enums stockés comme string en PostgreSQL (HasConversion<string>())
✅ IBlobStorageService existant réutilisé (pas de nouvelle implémentation)
✅ IImageProcessingService existant réutilisé (SkiaSharp)
✅ Container Blob : phoenix-customer-logos (PRIVATE, SAS 1h)
✅ Blob path relatif en DB : customers/{customerId}/logos/{jobId}/{filename}.png
✅ Refresh token hashé en DB (SHA256) — JAMAIS en clair
✅ Position logo en % (0-100) — indépendant résolution
✅ State machine stricte JobStatus (Draft→LogoUploaded→Confirmed, Draft→Cancelled)
✅ Prix recalculé côté serveur au Confirm (source de vérité)
✅ Validation job appartient au customer (via claims JWT) sur chaque endpoint
✅ Serilog logs structurés dans chaque handler
✅ traceId dans toutes les réponses d'erreur
✅ Swagger annoté sur tous les endpoints
✅ Testcontainers.PostgreSQL pour les tests d'intégration

Frontend Angular 21 :
✅ Konva.js via @defer — JAMAIS dans le bundle principal
✅ Canvas responsive via ResizeObserver
✅ Position logo pixels ↔ % conversion bidirectionnelle
✅ Calcul prix temps réel via computed() Signals
✅ Debounce 300ms sur position update API calls
✅ Upload logo : validation client (extension + taille) avant upload
✅ PrimeNG pour panneaux configuration + upload + formulaires
✅ CSS custom Phoenix #E8552A pour le configurateur public
✅ OnPush + trackBy sur toutes les boucles
✅ loading / error / empty states partout
✅ input() / output() — PAS @Input() / @Output()
✅ signal() / computed() — PAS BehaviorSubject
✅ inject() — PAS constructeur pour les injections
✅ standalone: true sur tous les composants
✅ withCredentials: true sur appels API
```

---

## FORMAT DE RÉPONSE

```
1. HYPOTHÈSES (avant tout code)
2. ARBORESCENCE (backend + frontend — montrer ce qui est AJOUTÉ)
3. DOMAIN (CustomizationJob, LogoFile, JobStatus, events, interfaces)
4. APPLICATION (commands, queries, DTOs, validators, PriceCalculationService)
5. INFRASTRUCTURE (EF Core configs, repository, migration, DbContext ajouts)
6. API (CustomizationsController + Swagger)
7. FRONTEND SERVICE (ConfiguratorService state machine + CustomizationApiService)
8. FRONTEND CANVAS (CanvasPreviewComponent Konva.js + @defer wrapper)
9. FRONTEND PANNEAUX (VariantSelector, PrintOptions, LogoUpload, Quantity, PriceSummary)
10. FRONTEND PAGES (ConfiguratorPage + CustomerJobsPage)
11. MODIFICATIONS MODULES EXISTANTS (bouton fiche produit + carte dashboard)
12. TESTS (unitaires + intégration)
13. COMMANDES D'EXÉCUTION
14. CHECKLIST DOD ← remplir case par case
```

---

## CHECKLIST DOD — À VALIDER AVANT MERGE

```
Code — Domain Customization
[ ] CustomizationJob agrégat avec méthodes métier (Create, UpdateLogo,
    UpdatePosition, Confirm, Cancel)
[ ] State machine stricte : Draft→LogoUploaded→Confirmed, Draft→Cancelled
[ ] Invariants : Quantity >= MOQ, TotalPriceHT > 0, Position 0-100%
[ ] LogoFile entité avec BlobPath relatif
[ ] JobStatus enum (Draft, LogoUploaded, Confirmed, Cancelled)
[ ] Events : Created, Confirmed, Cancelled
[ ] ICustomizationJobRepository

Code — Application
[ ] CreateJobCommand : valide IsCustomizable + MOQ + calcule prix
[ ] UpdateJobLogoCommand : upload Blob + validation format/taille + supprime ancien
[ ] UpdateJobPositionCommand : validation % 0-100
[ ] ConfirmJobCommand : recalcul prix serveur + transition Confirmed
[ ] CancelJobCommand : uniquement Draft + supprime blob
[ ] GetJobByIdQuery : retourne DTO + SAS URL logo
[ ] GetJobsByCustomerQuery : paginé + filtre statut
[ ] PriceCalculationService : palier × PrintSideCoeff × ColorCountCoeff
[ ] Chaque endpoint valide que le job appartient au customer (claims)

Code — Infrastructure
[ ] CustomizationJobConfiguration : precision decimal, enums string, index
[ ] LogoFileConfiguration
[ ] PhoenixDbContext : DbSet<CustomizationJob> + DbSet<LogoFile> ajoutés
[ ] Migration AddCustomizationModule générée
[ ] CustomizationJobRepository implémenté

Qualité
[ ] Tests unitaires CustomizationJob invariants OK (state machine complète)
[ ] Tests unitaires CreateJob/UpdateLogo/Confirm handlers OK
[ ] Tests unitaires PriceCalculationService OK (tous coefficients)
[ ] Tests intégration CustomizationsController Testcontainers.PostgreSQL OK
[ ] Lint Angular 0 erreur
[ ] Build .NET 0 warning

API/Contrats
[ ] POST /api/v1/customizations → 201 + CreateJobResponse [Customer]
[ ] PUT  /api/v1/customizations/{id}/logo → 200 multipart [Customer]
[ ] PUT  /api/v1/customizations/{id}/position → 200 [Customer]
[ ] POST /api/v1/customizations/{id}/confirm → 200 + prix final [Customer]
[ ] DELETE /api/v1/customizations/{id} → 204 (Draft only) [Customer]
[ ] GET  /api/v1/customizations/{id} → 200 + SAS URL logo [Customer]
[ ] GET  /api/v1/customizations/my-jobs → 200 + PaginatedList [Customer]
[ ] Swagger annoté avec exemples
[ ] Codes HTTP corrects (200, 201, 204, 400, 401, 403, 404)
[ ] Contrat erreur uniforme { code, message, details, traceId }

Sécurité
[ ] Chaque endpoint vérifie que le job appartient au customer connecté
[ ] Logo stocké dans container PRIVATE (SAS token 1h)
[ ] Validation format + taille côté serveur (pas seulement client)
[ ] Prix recalculé côté serveur au Confirm (pas confiance au client)

Base de données (PostgreSQL)
[ ] Migration AddCustomizationModule créée
[ ] Table CustomizationJob avec colonnes correctes
[ ] Table LogoFile avec FK vers CustomizationJob
[ ] Index sur CustomerId, Status
[ ] Enums stockés en string (HasConversion<string>())
[ ] Decimal precision (18,4) pour les prix

Blob Storage
[ ] Container phoenix-customer-logos (PRIVATE)
[ ] Blob path relatif : customers/{customerId}/logos/{jobId}/{filename}.png
[ ] SAS URL 1h pour accès lecture
[ ] Ancien logo supprimé quand remplacé
[ ] IBlobStorageService existant réutilisé (pas de duplication)

Frontend Angular 21 — Configurateur
[ ] CanvasPreviewComponent Konva.js chargé via @defer
[ ] Canvas responsive (ResizeObserver)
[ ] Logo positionnable (drag, resize, rotate) avec Konva.Transformer
[ ] Centrage automatique au premier placement
[ ] Vue recto/verso toggle (si DoubleSide)
[ ] Position en % (conversion pixels ↔ %)
[ ] Debounce 300ms sur position update API

Frontend Angular 21 — Panneaux & Service
[ ] ConfiguratorService : state machine complète via Signals
[ ] Calcul prix temps réel via computed() (coefficients identiques au Domain)
[ ] VariantSelectorPanel : dropdown variante + MOQ affiché
[ ] PrintOptionsPanel : RadioButtons recto/recto-verso + coefficients affichés
[ ] LogoUploadPanel : PrimeNG p-fileUpload + validation client
[ ] QuantityInput : p-inputNumber + min=MOQ + label pièces/KG
[ ] PriceSummaryPanel : UnitPriceHT, TotalHT, TVA, TotalTTC temps réel
[ ] ConfirmButton : disabled tant que logo absent ou qty < MOQ
[ ] JobStatusBadge : couleurs par statut

Frontend Angular 21 — Pages & Routing
[ ] ConfiguratorPage : layout responsive canvas + panneaux
[ ] CustomerJobsPage : liste paginée avec badges statut
[ ] Bouton "Personnaliser" ajouté sur fiche produit (Module 1)
[ ] Carte "Mes personnalisations" ajoutée au dashboard client (Module 2)
[ ] Routes protégées AuthGuard + RoleGuard("Customer")
[ ] Composants : input() / output() / signal() / inject() / standalone / OnPush
[ ] États loading / error / empty présents partout
[ ] CSS Phoenix #E8552A sur le configurateur
[ ] withCredentials: true sur appels API
```

---

## ORDRE D'EXÉCUTION RECOMMANDÉ (8 tours)

```
Tour 1 → Arborescence + Domain CustomizationJob + LogoFile + JobStatus enum
          + Events + ICustomizationJobRepository
Tour 2 → Application : Commands (CreateJob, UpdateJobLogo, UpdateJobPosition,
          ConfirmJob, CancelJob) + Queries (GetJobById, GetJobsByCustomer)
          + DTOs + Validators + IPriceCalculationService
Tour 3 → Infrastructure : EF Core configs (CustomizationJob, LogoFile)
          + Repository + PriceCalculationService
          + PhoenixDbContext ajouts + Migration AddCustomizationModule
Tour 4 → API : CustomizationsController (7 endpoints) + Swagger
Tour 5 → Angular : ConfiguratorService (state machine Signals)
          + CustomizationApiService (HTTP layer)
          + models + routes
Tour 6 → Angular : CanvasPreviewComponent (Konva.js via @defer)
          + ConfiguratorPage (layout responsive)
          + ResizeObserver + pixels ↔ % + Transformer
Tour 7 → Angular : Panneaux (VariantSelector, PrintOptions, LogoUpload,
          QuantityInput, PriceSummary, ConfirmButton, JobStatusBadge)
          + CustomerJobsPage + modifications Modules 1-2
Tour 8 → Tests unitaires (CustomizationJob invariants + handlers + PriceCalc)
          + Tests intégration (CustomizationsController)
          + Vérification DoD complète
```

---

## VARIANTE ULTRA RAPIDE

```
En respectant les standards Phoenix (02 à 09 + PHOENIX-domain.md),
génère le module Customization (Configurateur 2D) complet Angular 21 + .NET 10 avec :
- Clean Architecture CQRS/MediatR, EF Core 10, PostgreSQL (Npgsql)
- CustomizationJob agrégat avec state machine (Draft→LogoUploaded→Confirmed)
- LogoFile entité + upload Azure Blob container phoenix-customer-logos (PRIVATE, SAS 1h)
- IBlobStorageService + IImageProcessingService existants réutilisés (Modules 1)
- Position logo en % (0-100) indépendante de la résolution
- Prix calculé : PriceTier × PrintSideCoeff × ColorCountCoeff (Domain Module 1)
- Recalcul prix côté serveur au Confirm (source de vérité)
- Canvas Konva.js via @defer (lazy SSR-compatible), responsive ResizeObserver
- Drag/resize/rotate logo Konva.Transformer, centrage auto
- Vue recto/verso toggle, conversion pixels ↔ %
- ConfiguratorService signal-based (state machine + computed prix temps réel)
- Panneaux PrimeNG : variante, impression, logo upload, quantité, prix, confirm
- CustomerJobsPage liste paginée avec badges statut
- Endpoints : create, upload logo, position, confirm, cancel, get, my-jobs [Customer]
- Validation : IsCustomizable, MOQ, format logo, taille 20MB, ownership claims
- Angular 21 Signals, input()/output(), inject(), standalone, OnPush
- Migration AddCustomizationModule + index CustomerId/Status
- Tests xUnit + Testcontainers.PostgreSQL + Jest Angular
- États loading/error/empty, trackBy, debounce 300ms position, withCredentials
- Swagger complet, Serilog, traceId erreurs
Code production-ready, sans pseudo-code, sans placeholder.
```

---

## MODULES SUIVANTS (après Customization validé DoD)

```
Module 4 : Quote               (devis en ligne + PDF QuestPDF + BAT)
Module 5 : Order & Payment     (tunnel commande + Stripe webhooks
                                 + OrderLine.CustomizationJobId)
Module 6 : Admin Dashboard     (Kanban PrimeNG + charts + KPIs)
Module 7 : Notifications       (emails SendGrid transactionnels)
```

> Règle absolue : ne pas passer au module suivant avant DoD validé.
