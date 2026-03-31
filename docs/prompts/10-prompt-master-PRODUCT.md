# Prompt Master — Module Product & Catalog
# Phoenix Emballages — Sprint 1
# Prêt à coller dans Claude Code (onglet Code, Claude Desktop)

> VERSION  : 2.0 — Stack Angular 21 / .NET 10 / EF Core 10 / PostgreSQL
> MODULE   : Product & Catalog
> SPRINT   : 1 — Socle vertical de référence (Golden Path)
> DURÉE    : ~24h dev
> STATUT   : Prêt à exécuter — Gate Sprint 0 validé ✅

---

## ══════════════════════════════════════════
## PROMPT — COLLER DIRECTEMENT DANS CLAUDE CODE
## ══════════════════════════════════════════

Tu es un Lead Full Stack Angular 21 / ASP.NET Core 10 expert en Clean Architecture,
CQRS/MediatR, EF Core 10, PostgreSQL, Azure Blob Storage et PrimeNG.

Tu génères du code **production-ready**, **complet**, **sans pseudo-code**, sans placeholder.
Chaque fichier est livré en entier. Zéro instruction "à compléter".

---

## CONTEXTE PROJET

- **Produit**    : Phoenix Emballages — E-commerce B2B d'emballages alimentaires personnalisables
- **Type**       : B2B + B2C — site public + back-office admin
- **Stack back** : .NET 10, ASP.NET Core 10, EF Core 10, **PostgreSQL** (Npgsql), MediatR,
                   FluentValidation, AutoMapper, Azure Blob Storage, SkiaSharp, QuestPDF, Serilog
- **Stack front**: Angular 21 standalone SSR, PrimeNG 18, Angular Signals, Reactive Forms
- **Hébergement**: Azure App Service Linux + Azure Database for PostgreSQL Flexible Server
                   + Azure Blob Storage + Azure CDN
- **Auth**       : ASP.NET Identity + JWT Bearer
- **Langue code**: EN — Labels UI : FR

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
- `PHOENIX-domain.md`          → Ubiquitous language, SKUs réels, MOQ variable, SoldByWeight, flags

---

## OBJECTIF DU MODULE

Générer le module **Product & Catalog** complet.
C'est le **SOCLE** de toute l'application — aucun autre module ne peut exister sans lui.
Il doit être parfait et servir de GOLDEN PATH réutilisable pour tous les modules suivants.

### Périmètre fonctionnel

**Site public (non authentifié) :**
- Catalogue produits avec filtres : famille, segment métier, isCustomizable, recherche texte
- Navigation par métier (FastFood, Boulangerie, Japonais, BubbleTea, FoodTruck...)
- Fiche produit : variantes, galerie photos, tableau paliers de prix, MOQ, badges
- Badges : Personnalisable, Gamme Gourmet, Éco, Agréé Contact Alimentaire, Express 24h
- Promotion "5 achetés = 1 offert" affichée si applicable

**Back-office admin (Employee + Admin) :**
- Liste paginée PrimeNG DataTable avec tri/filtre/recherche
- CRUD produit complet avec variantes et paliers de prix
- Upload image produit → Azure Blob Storage → WebP via SkiaSharp
- Activation/désactivation (IsActive — soft delete logique)

---

## SPÉCIFICATIONS MÉTIER IMPÉRATIVES (PHOENIX-domain.md)

### Règles métier Product

```
1. MOQ VARIABLE PAR SKU
   Chaque ProductVariant a son propre MinimumOrderQuantity
   Validation : quantity >= productVariant.MinimumOrderQuantity
   Exemples réels : 50 pcs (Pizza T40), 200 pcs (Gourmet Sandwich), 500 pcs (Sac SOS)

2. VENDU AU POIDS (SoldByWeight)
   Product.SoldByWeight = true → papiers ingraissables
   Quand true : quantité saisie en KG, pas en pièces
   UI affiche "par KG" au lieu de "pièces"

3. PALIERS DE PRIX (PriceTier)
   Chaque ProductVariant → liste ordonnée de PriceTier
   Structure : { MinQuantity, MaxQuantity (nullable), UnitPriceHT }
   Le prix affiché = meilleur palier pour la quantité saisie
   Si quantity < MOQ → erreur de validation, bloquer l'ajout panier

4. COEFFICIENTS D'IMPRESSION (produits personnalisables uniquement)
   PrintSide.SingleSide  → x1.00 (base)
   PrintSide.DoubleSide  → x1.15 (+15% — EXCLUSIF Phoenix, différenciateur clé)
   ColorCount.One        → x1.00
   ColorCount.Two        → x1.10
   ColorCount.Three      → x1.18
   ColorCount.FourCMYK   → x1.25

5. FLAGS PRODUIT (7 booléens sur entité Product)
   IsCustomizable     → peut recevoir impression logo client
   IsGourmetRange     → appartient à la Gamme Gourmet premium
   IsBulkOnly         → grandes séries uniquement, non personnalisable
   IsEcoFriendly      → certifié biodégradable/écologique
   IsFoodApproved     → agréé Contact Alimentaire
   SoldByWeight       → vendu au KG
   HasExpressDelivery → éligible livraison express 24h IDF
   IsActive           → visible/disponible (soft delete logique)

6. PROMOTION 5+1
   PromotionRule entité configurable en admin
   Badge affiché sur fiche produit si règle active
```

### Enums domaine

```csharp
// ProductFamily (27 valeurs)
KraftBagHandled, KraftBagSOS, GourmetRange, KraftBowl, GreaseproofPaper,
ChirashiBowl, HingedTray, SushiTray, MicroTray, BioPack, PastaPouch,
SoupPouch, SaucePouch, FriesCone, MilkshakeCup, CoffeeCup, DessertPot,
PizzaBox, GreaseproofBag, SandwichBag, ReusableBag, Napkin, WoodenCutlery,
Bottle, GarbageBag, FoodWrap, HygieneMisc

// CustomerSegment (10 valeurs)
FastFood, BakeryPastry, JapaneseAsian, BubbleTea, RetailCommerce,
FoodTruck, Catering, ChocolateConfectionery, PizzaShop, Other

// PrintSide
SingleSide, DoubleSide

// ColorCount
One, Two, Three, FourCMYK
```

### Permissions

```
GET  /api/v1/products           → Public (non authentifié)
GET  /api/v1/products/{id}      → Public
GET  /api/v1/products/families  → Public
POST /api/v1/admin/products                  → [Authorize("Admin,Employee")]
PUT  /api/v1/admin/products/{id}             → [Authorize("Admin,Employee")]
DELETE /api/v1/admin/products/{id}           → [Authorize("Admin")]
POST /api/v1/admin/products/{id}/image       → [Authorize("Admin,Employee")]
```

---

## LIVRABLES ATTENDUS — OBLIGATOIRES

### 1. Arborescence complète

Donner l'arborescence complète backend + frontend AVANT tout code.

### 2. Domain Layer — code complet

```
Phoenix.Domain/Products/
  Entities/
    Product.cs                  (agrégat racine avec tous les flags)
    ProductVariant.cs           (MOQ, PriceTiers, PrintSide, ColorCount)
    PriceTier.cs                (MinQuantity, MaxQuantity nullable, UnitPriceHT)
    ProductImage.cs             (BlobPath relatif, PublicUrl CDN, IsMain)
  ValueObjects/
    ProductFamily.cs            (enum 27 valeurs)
    CustomerSegment.cs          (enum 10 valeurs)
    PrintSide.cs                (enum SingleSide | DoubleSide)
    ColorCount.cs               (enum One | Two | Three | FourCMYK)
    Money.cs                    (Amount decimal, Currency = "EUR")
  Events/
    ProductCreatedEvent.cs
    ProductUpdatedEvent.cs
    ProductDeactivatedEvent.cs
  Repositories/
    IProductRepository.cs
```

### 3. Application Layer — code complet

```
Phoenix.Application/Products/
  Commands/
    CreateProduct/
      CreateProductCommand.cs + Handler + Validator
    UpdateProduct/
      UpdateProductCommand.cs + Handler + Validator
    DeactivateProduct/
      DeactivateProductCommand.cs + Handler
    UploadProductImage/
      UploadProductImageCommand.cs + Handler
      (Handler appelle IBlobStorageService + SkiaSharp via IImageProcessingService)
  Queries/
    GetProductList/
      GetProductListQuery.cs    (filtres: family, segment, isCustomizable, search, page, pageSize, sortBy, sortDir)
      GetProductListQueryHandler.cs
      ProductSummaryDto.cs
    GetProductById/
      GetProductByIdQuery.cs
      GetProductByIdQueryHandler.cs
      ProductDetailDto.cs       (avec variantes, paliers, images)
    GetProductFamilies/
      GetProductFamiliesQuery.cs + Handler
      ProductFamilyDto.cs
  Dtos/
    ProductSummaryDto.cs        (pour la liste catalogue public)
    ProductDetailDto.cs         (pour la fiche produit complète)
    ProductVariantDto.cs
    PriceTierDto.cs
    CreateProductRequest.cs
    UpdateProductRequest.cs
    UploadProductImageResponse.cs (mainUrl, thumbUrl)
  Mappings/
    ProductMappingProfile.cs
```

### 4. Infrastructure Layer — code complet

```
Phoenix.Infrastructure/
  Persistence/
    Configurations/
      ProductConfiguration.cs          (IEntityTypeConfiguration<Product>)
      ProductVariantConfiguration.cs
      PriceTierConfiguration.cs
      ProductImageConfiguration.cs
    PhoenixDbContext.cs                 (NpgsqlDbContext — DbSet<Product>, etc.)
    Migrations/                         (EF Core 10 migration générée)
    DataSeed/
      ProductDataSeed.cs               (10 produits réels du catalogue)
  Storage/
    AzureBlobStorageService.cs         (IBlobStorageService — Azure SDK v12)
    ImageProcessingService.cs          (IImageProcessingService — SkiaSharp resize + WebP)
  Repositories/
    ProductRepository.cs               (IProductRepository)
```

#### Règles EF Core 10 / PostgreSQL

```csharp
// Dans ProductConfiguration.cs — exemple de contraintes
builder.Property(p => p.Sku).HasMaxLength(50).IsRequired();
builder.HasIndex(p => p.Sku).IsUnique();
builder.HasIndex(p => p.ProductFamily);
builder.HasIndex(p => p.IsActive);
builder.HasIndex(p => p.IsCustomizable);
// PostgreSQL : utiliser HasColumnType("text") pour les strings longues
// PostgreSQL : les enums EF Core → stocker comme string (HasConversion<string>())
```

### 5. API Layer — code complet

```
Phoenix.Api/Controllers/v1/
  ProductsController.cs
    GET /api/v1/products                → GetProductListQuery
    GET /api/v1/products/{id}           → GetProductByIdQuery
    GET /api/v1/products/families       → GetProductFamiliesQuery

Phoenix.Api/Controllers/v1/Admin/
  AdminProductsController.cs
    POST   /api/v1/admin/products       → CreateProductCommand
    PUT    /api/v1/admin/products/{id}  → UpdateProductCommand
    DELETE /api/v1/admin/products/{id}  → DeactivateProductCommand
    POST   /api/v1/admin/products/{id}/image → UploadProductImageCommand
```

### 6. Frontend Angular 21 — code complet

```
src/app/features/catalog/
  pages/
    catalog.page.ts                    (smart — state via Signals)
    product-detail.page.ts             (smart)
  components/
    product-card/product-card.component.ts        (dumb, input())
    product-filters/product-filters.component.ts  (dumb, output())
    product-badge/product-badge.component.ts      (dumb, input())
    price-tier-display/price-tier-display.component.ts  (dumb)
    variant-selector/variant-selector.component.ts      (dumb)
  services/
    product-catalog.service.ts         (signal() pour state, HttpClient)
  models/
    product.model.ts / product-filters.model.ts
  catalog.routes.ts

src/app/features/admin/products/
  pages/
    admin-product-list.page.ts         (smart — PrimeNG p-table)
    admin-product-form.page.ts         (smart — create + edit)
  components/
    price-tier-editor/price-tier-editor.component.ts
    variant-manager/variant-manager.component.ts
    product-image-uploader/product-image-uploader.component.ts
  services/
    admin-product.service.ts
  admin-products.routes.ts
```

#### Angular 21 — contraintes obligatoires

```typescript
// Tous les composants : standalone: true, changeDetection: OnPush
// Inputs : input() à la place de @Input()
// Outputs : output() à la place de @Output()
// Services : signal() / computed() pour le state (plus de BehaviorSubject)
// Configurateur lourd : @defer { <app-canvas-preview/> }
// inject() à la place du constructeur
// trackBy sur tout ngFor
// loading / error / empty states sur toutes les listes

// Exemple service avec Signals :
@Injectable({ providedIn: 'root' })
export class ProductCatalogService {
  private readonly http = inject(HttpClient);

  products = signal<ProductSummary[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  totalCount = computed(() => this.products().length);

  loadProducts(filters: ProductFilters): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.http.get<PaginatedList<ProductSummary>>('/api/v1/products', { params: ... })
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (res) => this.products.set(res.items),
        error: (err) => this.error.set(err.message)
      });
  }
}
```

### 7. Seed data — 10 produits représentatifs (réels du catalogue)

```csharp
// ProductDataSeed.cs — données réelles du catalogue Phoenix 2023
// Produit 1  : Sac Kraft Brun 22x10x28 — IsCustomizable=true, MOQ=250
// Produit 2  : Sac Kraft Blanc 22x10x28 — IsCustomizable=true, MOQ=500
// Produit 3  : Gourmet Burger S — IsCustomizable=true, IsGourmetRange=true, MOQ=600
// Produit 4  : Gourmet Burger L — IsCustomizable=true, IsGourmetRange=true, MOQ=500
// Produit 5  : Gourmet Wrap L — IsCustomizable=true, IsGourmetRange=true, MOQ=800
// Produit 6  : Bol Kraft 500cc — IsCustomizable=true, MOQ=300
// Produit 7  : Papier Ingraissable 25x35 — IsCustomizable=true, SoldByWeight=true, MOQ=1000pcs/10kg
// Produit 8  : Barquette Charnière 500ml — IsBulkOnly=true, MOQ=900
// Produit 9  : Barquette Sushi SM02 — IsBulkOnly=true, MOQ=400
// Produit 10 : Boîte Pizza T29 — IsBulkOnly=true, MOQ=100

// Chaque produit seed doit avoir :
// - 1 ProductVariant minimum avec MOQ correct
// - 3 PriceTier minimum (paliers de quantité croissants)
// - Tous les flags booléens corrects
// - IsActive = true
```

### 8. Tests

```
Phoenix.UnitTests/Products/
  ProductTests.cs                           (invariants domaine : MOQ, PriceTier, flags)
  CreateProductCommandHandlerTests.cs
  GetProductListQueryHandlerTests.cs
  UploadProductImageCommandHandlerTests.cs  (mock IBlobStorageService)

Phoenix.IntegrationTests/Products/
  ProductsControllerTests.cs               (Testcontainers.PostgreSQL)
    → GET /api/v1/products (pagination, filtres)
    → GET /api/v1/products/{id}
    → POST /api/v1/admin/products (auth Admin)
    → POST /api/v1/admin/products/{id}/image (auth Employee)
```

### 9. Commandes d'exécution

```bash
# ── Backend ────────────────────────────────────────────────
dotnet restore
dotnet build --configuration Release -warnaserror

# Migration PostgreSQL (EF Core 10 + Npgsql)
dotnet ef migrations add AddProductModule \
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

Backend :
✅ Npgsql — PAS de SqlServer provider
✅ Enums stockés comme string en PostgreSQL (HasConversion<string>())
✅ SkiaSharp.NativeAssets.Linux inclus (obligatoire Azure App Service Linux)
✅ IBlobStorageService : chemin RELATIF en DB, URL CDN construite dynamiquement
✅ PriceTier.MaxQuantity nullable (dernier palier = sans limite)
✅ Serilog logs structurés dans chaque handler
✅ traceId dans toutes les réponses d'erreur
✅ Swagger annoté sur tous les endpoints
✅ Testcontainers.PostgreSQL pour les tests d'intégration

Frontend Angular 21 :
✅ PrimeNG p-table + pagination côté serveur pour la liste admin
✅ PrimeNG p-fileupload pour l'upload image
✅ Site public : CSS custom Phoenix #E8552A — JAMAIS style PrimeNG brut visible
✅ Admin : PrimeNG librement utilisé
✅ OnPush + trackBy sur toutes les boucles
✅ loading / error / empty states partout
✅ input() / output() — PAS @Input() / @Output()
✅ signal() / computed() — PAS BehaviorSubject
✅ inject() — PAS constructeur pour les injections
✅ standalone: true sur tous les composants
```

---

## FORMAT DE RÉPONSE

```
1. HYPOTHÈSES (avant tout code)
2. ARBORESCENCE (backend + frontend)
3. DOMAIN (entités, value objects, interfaces)
4. APPLICATION (commands, queries, DTOs, validators, mappings)
5. INFRASTRUCTURE (EF Core config, repositories, Blob service, Image service, migrations)
6. API (controllers)
7. FRONTEND (services, models, composants, pages)
8. SEED DATA
9. TESTS
10. COMMANDES D'EXÉCUTION
11. CHECKLIST DOD ← remplir case par case
```

---

## CHECKLIST DOD — À VALIDER AVANT MERGE

```
Code
[ ] Product, ProductVariant, PriceTier, ProductImage créés
[ ] 7 flags booléens présents sur Product
[ ] MOQ sur ProductVariant (pas hardcodé)
[ ] PriceTier.MaxQuantity nullable
[ ] Coefficients recto-verso et couleurs dans le domaine
[ ] IBlobStorageService dans Domain/Common/Interfaces
[ ] AzureBlobStorageService implémenté dans Infrastructure
[ ] SkiaSharp.NativeAssets.Linux dans le .csproj

Qualité
[ ] Tests unitaires handlers Products OK
[ ] Tests intégration Testcontainers.PostgreSQL OK
[ ] Lint Angular 0 erreur
[ ] Build .NET 0 warning

API/Contrats
[ ] GET /api/v1/products avec pagination + filtres (family, segment, isCustomizable, search)
[ ] GET /api/v1/products/{id} avec variantes et paliers
[ ] POST /api/v1/admin/products protégé Admin+Employee
[ ] POST /api/v1/admin/products/{id}/image → Blob + WebP
[ ] Swagger annoté avec exemples
[ ] Codes HTTP corrects (200, 201, 400, 401, 403, 404)
[ ] Contrat erreur uniforme { code, message, details, traceId }

Base de données (PostgreSQL)
[ ] Migration EF Core 10 + Npgsql créée
[ ] Index sur ProductFamily, IsActive, IsCustomizable, Sku (unique)
[ ] Enums stockés en string (HasConversion<string>())
[ ] Seed 10 produits avec MOQ et PriceTiers réels du catalogue

Blob Storage
[ ] Chemin RELATIF stocké en DB (pas URL complète)
[ ] URL CDN construite dynamiquement via BlobSettings.CdnBaseUrl
[ ] Images converties en WebP avant upload
[ ] Validation format + taille côté serveur

Frontend Angular 21
[ ] CatalogPage : liste avec filtres (famille, segment, personnalisable)
[ ] ProductDetailPage : fiche complète + variantes + paliers + badges
[ ] AdminProductListPage : PrimeNG Table paginée + tri + recherche
[ ] AdminProductFormPage : création + édition complètes + upload image
[ ] Composants : input() / output() / signal() / inject() / standalone
[ ] États loading / error / empty présents partout
[ ] OnPush + trackBy sur toutes les listes
[ ] CSS Phoenix sur le site public (pas style PrimeNG brut)
```

---

## ORDRE D'EXÉCUTION RECOMMANDÉ (9 tours)

```
Tour 1 → Arborescence + entités Domain + IBlobStorageService interface
Tour 2 → Commands + Queries Application layer + DTOs + Validators
Tour 3 → EF Core configurations PostgreSQL + migrations + seed 10 produits
Tour 4 → AzureBlobStorageService + ImageProcessingService (SkiaSharp)
Tour 5 → Controllers API (public + admin) + Swagger
Tour 6 → Services Angular + models
Tour 7 → Composants Angular catalog (CatalogPage + ProductCard + Filters)
Tour 8 → Pages admin Angular (AdminProductList + AdminProductForm + PrimeNG)
Tour 9 → Tests unitaires + intégration + vérification DoD complète
```

---

## VARIANTE ULTRA RAPIDE

```
En respectant les standards Phoenix (02 à 09 + PHOENIX-domain.md),
génère le module Product complet Angular 21 + .NET 10 avec :
- Clean Architecture CQRS/MediatR, EF Core 10, PostgreSQL (Npgsql)
- Entités Product/ProductVariant/PriceTier avec tous les flags métier Phoenix
- IBlobStorageService (Azure Blob) + SkiaSharp (WebP) pour images produits
- CRUD admin + catalogue public paginé avec filtres famille/segment
- PrimeNG DataTable admin, CSS custom Phoenix site public
- Angular 21 Signals, input()/output(), inject(), standalone, OnPush
- Migration EF Core 10 PostgreSQL + seed 10 produits réels du catalogue
- Tests xUnit + Testcontainers.PostgreSQL + Jest Angular
- États loading/error/empty, trackBy
- Swagger complet, Serilog, traceId erreurs
Code production-ready, sans pseudo-code, sans placeholder.
```

---

## MODULES SUIVANTS (après Product validé DoD)

```
Module 2 : Auth & Customer     (ASP.NET Identity, JWT, espace client)
Module 3 : Customization       (configurateur 2D Konva.js + upload logo Blob)
Module 4 : Quote               (devis en ligne + PDF QuestPDF)
Module 5 : Order & Payment     (tunnel commande + Stripe webhooks)
Module 6 : Admin Dashboard     (Kanban PrimeNG + charts + KPIs)
Module 7 : Notifications       (emails SendGrid transactionnels)
```

> Règle absolue : ne pas passer au module suivant avant DoD validé.