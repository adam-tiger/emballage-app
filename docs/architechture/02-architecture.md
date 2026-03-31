# Architecture — Phoenix Emballages

> Version 3.0 — Stack : Angular 21 / .NET 10 / EF Core 10 / PostgreSQL
> Décisions validées — Ne pas modifier sans ADR.

---

## 1) Style d'architecture

- **Backend**  : Clean Architecture — Domain / Application / Infrastructure / API.
- **Frontend** : Feature-first Angular 21 avec couche partagée (`core`, `shared`, `features`).
- **Communication** : REST JSON + versioning `/api/v1/`.
- **Rendu**    : Angular 21 SSR — obligatoire pour SEO (landing pages métier indexées).
- **Auth**     : ASP.NET Identity + JWT Bearer tokens.

---

## 2) Stack technique décidée

### Backend

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Framework | ASP.NET Core | **10** | LTS, performance maximale |
| ORM | Entity Framework Core | **10** | Code-first, migrations, Npgsql natif |
| Base de données | **PostgreSQL** | 16+ | Open-source, coût Azure réduit, JSONB, full-text search natif |
| Driver DB | Npgsql EF Core | 10.x | Provider officiel PostgreSQL pour EF Core 10 |
| Validation | FluentValidation | 11+ | Standard Clean Archi |
| Mapping | AutoMapper | 13+ | Réduction boilerplate DTO ↔ Entity |
| Patterns | CQRS + MediatR | 12+ | Séparation commands/queries, testabilité |
| Logging | Serilog + Azure Application Insights | latest | Logs structurés, corrélation traceId |
| Auth | ASP.NET Identity + JWT | .NET 10 | Standard, extensible |
| Stockage fichiers | **Azure Blob Storage** | SDK v12 | Images produits, logos clients, PDFs |
| CDN images | **Azure CDN** | — | Images produits depuis edge node — LCP < 1s |
| Traitement images | **SkiaSharp** | 3.x | Resize + conversion WebP, cross-platform Linux |
| Emails | SendGrid | latest | Fiabilité, templates, analytics |
| Paiement | Stripe (V1) + Alma (V2) | latest | Stripe webhooks fiables |
| PDF | QuestPDF | 2024.x | Fluent API, open-source, BAT/devis/factures |
| Tests | xUnit + FluentAssertions + Testcontainers.PostgreSQL | latest | Vrai PostgreSQL en Docker pour les tests d'intégration |

### Frontend

| Composant | Choix | Version | Justification |
|---|---|---|---|
| Framework | Angular | **21** | Signals stables, SSR amélioré, defer blocks |
| Rendu | Angular SSR | 21 | SEO critique |
| UI Library | **PrimeNG** | 18+ | Back-office data-heavy, DataTable, formulaires riches |
| Canvas | Konva.js | latest | Configurateur 2D — léger, API intuitive |
| State | **Angular Signals** | natif 21 | `signal()` / `computed()` — remplace RxJS BehaviorSubject |
| Formulaires | Reactive Forms | natif 21 | Validation complexe |
| HTTP | `HttpClient` + Interceptors | natif 21 | Auth JWT, gestion erreurs centralisée |
| Tests | Jest + Angular Testing Library | latest | Rapide, bonne DX |
| Build | esbuild (Angular CLI 21) | natif | Ultra-rapide, tree-shaking agressif |

> **Angular 21 — patterns obligatoires dans tout le code généré :**
> - `input()` / `output()` à la place de `@Input()` / `@Output()`
> - `signal()` / `computed()` dans les services (plus de BehaviorSubject)
> - `@defer` pour les composants lourds (configurateur Konva.js)
> - Tous les composants `standalone: true` — pas de NgModules
> - `inject()` à la place du constructeur pour les injections de dépendances

### Infrastructure Azure

```
Azure App Service (Linux P1v3)       → Backend ASP.NET Core 10
Azure static webapp                  → Frontend Angular 21 SSR
Azure Database for PostgreSQL        → Flexible Server — General Purpose 2 vCores
Azure Blob Storage (3 containers)    → Fichiers binaires (voir section 3)
Azure CDN                            → Frontal images produits (TTL 7 jours)
Azure Application Insights           → Monitoring, traces, alertes
Azure Key Vault                      → Tous les secrets
GitHub Actions                       → CI/CD
```

---

## 3) Architecture Blob Storage — détail complet

> Standard de l'industrie pour Angular/.NET sur Azure.
> JAMAIS en base de données. JAMAIS dans le filesystem App Service (éphémère au restart).

### 3.1) Les trois containers

```
phoenix-product-images   → accès PUBLIC  → images catalogue (URL CDN directe)
phoenix-customer-logos   → accès PRIVÉ   → logos clients (SAS token 1h)
phoenix-documents        → accès PRIVÉ   → PDFs BAT/devis/factures (SAS token 15min)
```

### 3.2) Structure des chemins dans chaque container

```
phoenix-product-images/
  products/{productId}/
    main.webp              → image principale (800px max)
    thumb.webp             → miniature (400px max)
    gallery-1.webp         → galerie optionnelle

phoenix-customer-logos/
  {customerId}/{jobId}/
    original.svg           → fichier original conservé intact
    processed.png          → version PNG 300 DPI pour le configurateur

phoenix-documents/
  orders/{orderId}/
    bat-v1.pdf | bat-v2.pdf | invoice.pdf
  quotes/{quoteId}/
    quote.pdf
```

### 3.3) Règles d'accès

```
Images produits  → URL publique via Azure CDN
                   https://cdn.phoenix-emballages.fr/products/{id}/main.webp

Logos clients    → SAS token signé côté .NET, durée 1h max

Documents PDF    → SAS token signé côté .NET, durée 15min max
                   Jamais d'URL permanente pour documents sensibles
```

### 3.4) Pipeline upload image produit

```
[Angular p-fileupload]
  │ validation client : format jpg/png/webp, taille < 5MB
  │
  POST /api/v1/admin/products/{id}/image
  │
  [ASP.NET Core]
  ├─ Validation : format, taille, dimensions min 400×400px
  ├─ SkiaSharp  : resize → 800px max (main) + 400px max (thumb)
  ├─ SkiaSharp  : conversion → WebP qualité 85% / 80%
  ├─ Nom Blob   : products/{productId}/main-{timestamp}.webp
  ├─ Upload     → Azure Blob phoenix-product-images
  ├─ DB update  : Product.ImagePath = chemin RELATIF uniquement
  └─ Retour     : { mainUrl (CDN), thumbUrl (CDN) }
```

### 3.5) Règle fondamentale — ce qu'on stocke en DB

```sql
-- JAMAIS l'URL complète Azure en base → si on change de région tout casse
-- TOUJOURS le chemin relatif
Product.ImagePath = 'products/abc123/main-1706000000.webp'

-- L'URL complète est construite dynamiquement par le service :
-- BlobSettings.CdnBaseUrl + "/" + Product.ImagePath
```

### 3.6) IBlobStorageService — dans Domain/Common/Interfaces

```csharp
public interface IBlobStorageService
{
    Task<BlobUploadResult> UploadProductImageAsync(
        Guid productId, Stream stream, string fileName, CancellationToken ct = default);

    Task<BlobUploadResult> UploadCustomerLogoAsync(
        Guid customerId, Guid jobId, Stream stream, string fileName, CancellationToken ct = default);

    Task<BlobUploadResult> UploadDocumentAsync(
        string containerPath, Stream stream, string fileName, CancellationToken ct = default);

    Task<string> GenerateSasUrlAsync(
        string containerName, string blobPath, TimeSpan duration, CancellationToken ct = default);

    Task DeleteAsync(string containerName, string blobPath, CancellationToken ct = default);
}

public record BlobUploadResult(
    string BlobPath,     // relatif → stocké en DB
    string PublicUrl,    // URL CDN complète → retourné au client
    string? ThumbPath,
    string? ThumbUrl);
```

### 3.7) NuGet Blob + SkiaSharp (obligatoires)

```xml
<PackageReference Include="Azure.Storage.Blobs" Version="12.*" />
<PackageReference Include="SkiaSharp" Version="3.*" />
<PackageReference Include="SkiaSharp.NativeAssets.Linux" Version="3.*" />
<!-- SkiaSharp.NativeAssets.Linux : OBLIGATOIRE pour Azure App Service Linux -->
```

---

## 4) Découpage des couches backend

```
Phoenix.Domain/
  ├── Products/      Entities/, ValueObjects/, Events/, Repositories/
  ├── Orders/        Entities/, ValueObjects/, Events/, Repositories/
  ├── Quotes/        Entities/, ValueObjects/, Repositories/
  ├── Customers/     Entities/, ValueObjects/, Repositories/
  ├── Customizations/Entities/, ValueObjects/, Repositories/
  └── Common/
      ├── Interfaces/ IUnitOfWork, ICurrentUserService, IDateTimeService,
      │               IBlobStorageService, IEmailService
      └── Exceptions/ DomainException, BusinessRuleException

Phoenix.Application/
  ├── Products/      Commands/, Queries/, Dtos/, Validators/, Mappings/
  ├── Orders/        Commands/, Queries/, Dtos/, Validators/
  ├── Quotes/        Commands/, Queries/, Dtos/
  ├── Customers/     Commands/, Queries/, Dtos/
  ├── Customizations/Commands/, Queries/, Dtos/
  ├── Payments/      Commands/, Services/
  ├── Notifications/ Services/
  └── Common/        Behaviors/ (Validation, Logging, Performance)

Phoenix.Infrastructure/
  ├── Persistence/
  │   ├── PhoenixDbContext.cs        (NpgsqlDbContext)
  │   ├── Migrations/
  │   └── Configurations/            (IEntityTypeConfiguration<T>)
  ├── Storage/
  │   ├── AzureBlobStorageService.cs
  │   └── ImageProcessingService.cs  (SkiaSharp)
  ├── Email/         SendGridEmailService.cs
  ├── Payment/       StripeService.cs
  ├── Repositories/  ProductRepository.cs, OrderRepository.cs, ...
  └── Identity/      ApplicationUser.cs

Phoenix.Api/
  ├── Controllers/v1/        ProductsController (public)
  ├── Controllers/v1/Admin/  AdminProductsController, AdminOrdersController, ...
  ├── Middleware/             ExceptionHandlingMiddleware, CorrelationIdMiddleware
  └── Extensions/            ServiceCollectionExtensions
```

---

## 5) Découpage frontend Angular 21

```
src/app/
  ├── core/
  │   ├── auth/       auth.service.ts (Signals), auth.guard.ts, jwt.interceptor.ts
  │   ├── http/       api.service.ts, error.interceptor.ts
  │   └── config/     app.config.ts
  ├── shared/
  │   ├── ui/         button, badge, loading-spinner, empty-state
  │   ├── pipes/      currency-eur.pipe.ts, order-status.pipe.ts
  │   └── models/     api-response.model.ts, paginated-list.model.ts
  └── features/
      ├── home/
      ├── catalog/
      ├── configurator/    (@defer sur Konva.js)
      ├── cart/
      ├── payment/
      ├── quotes/
      ├── customer/
      ├── auth/
      └── admin/
          ├── dashboard/
          ├── orders/      (Kanban PrimeNG)
          ├── quotes/
          ├── products/    (CRUD + upload images)
          └── customers/
```

---

## 6) Règles de dépendances

- `API` → `Application` → `Domain` (jamais l'inverse)
- `Infrastructure` → `Application` + `Domain`
- `Domain` ne dépend d'aucun package externe
- `IBlobStorageService` : défini dans `Domain/Common/Interfaces`, implémenté dans `Infrastructure`
- Front `feature` → `shared` / `core` — jamais feature → feature directement

---

## 7) Patterns imposés

| Catégorie | Pattern | Implémentation |
|---|---|---|
| Commands/Queries | CQRS | MediatR — un handler par use case |
| Validation | Pipeline | FluentValidation + MediatR ValidationBehavior |
| Mapping | Centralisé | AutoMapper profiles par feature |
| Erreurs API | Standard uniforme | `{ code, message, details, traceId }` |
| Logging | Structuré | Serilog + corrélation traceId |
| Transactions | Unit of Work | Par use case modifiant l'état |
| Auth | JWT Bearer | Refresh token via cookie HttpOnly |
| Images produits | URL publique CDN | Azure CDN + Blob (container public) |
| Logos/PDFs privés | SAS Token | 1h logos / 15min PDFs |
| Stockage path | Chemin relatif en DB | URL construite dynamiquement |
| Images output | WebP via SkiaSharp | Conversion avant upload |
| Pagination | Serveur | `{ items, page, pageSize, totalCount, totalPages }` |
| Soft delete | Flag | `IsActive` / `IsDeleted` + `DeletedAtUtc` |
| State Angular | Signals natifs | `signal()` / `computed()` — pas de NgRx |

---

## 8) Décisions ADR

| ADR | Décision | Statut |
|---|---|---|
| ADR-001 | Angular 21 SSR pour SEO | ✅ |
| ADR-002 | PrimeNG UI Library (Angular 21 compatible) | ✅ |
| ADR-003 | Konva.js configurateur 2D + @defer | ✅ |
| ADR-004 | Stripe V1 + Alma V2 | ✅ |
| ADR-005 | **PostgreSQL** — Azure Database for PostgreSQL Flexible Server | ✅ |
| ADR-006 | JWT + ASP.NET Identity | ✅ |
| ADR-007 | Azure Blob Storage (3 containers) + Azure CDN | ✅ |
| ADR-008 | QuestPDF génération PDF | ✅ |
| ADR-009 | MediatR CQRS | ✅ |
| ADR-010 | **.NET 10 + EF Core 10** | ✅ |
| ADR-011 | **Angular Signals** comme state management | ✅ |
| ADR-012 | **SkiaSharp** resize + WebP | ✅ |

---

## 9) Notes PostgreSQL — points d'attention pour EF Core 10

```
Provider    : Npgsql.EntityFrameworkCore.PostgreSQL (remplace SqlServer)
Connection  : Host=...;Database=phoenix;Username=...;Password=...

Différences vs SQL Server :
  ✅ GUID         : uuid natif PostgreSQL — EF Core 10 gère automatiquement
  ✅ JSONB        : stockage flexible métadonnées produit (specs techniques variables)
  ✅ Full-text    : tsvector natif — recherche produits sans Elasticsearch
  ⚠️ Case-sensitive par défaut → utiliser citext ou ILIKE pour les recherches
  ⚠️ Dates        : PostgreSQL utilise timestamptz — configurer UseTimestamptz() dans Npgsql
  ⚠️ Migrations   : relancer dotnet ef migrations add après changement de provider

Testcontainers :
  Testcontainers.PostgreSQL démarre un vrai container Docker PostgreSQL
  → Les tests d'intégration testent le vrai comportement PostgreSQL
  → Plus fiable que SQLite en mémoire
```

---

## 10) Niveaux de test

| Couche | Type | Outil | Couverture cible |
|---|---|---|---|
| Domain | Unitaire | xUnit + FluentAssertions | 80% |
| Application (handlers) | Unitaire | xUnit + Moq | 70% |
| API (endpoints) | Intégration | WebApplicationFactory + **Testcontainers.PostgreSQL** | Cas critiques |
| Angular | Unitaire | Jest + Angular Testing Library | Composants clés |
| E2E | Parcours critiques | Playwright | Commande + Paiement + Admin Kanban |

---

## 11) Non-fonctionnel

| Critère | Cible | Mesure |
|---|---|---|
| Disponibilité | 99.5% | Azure Monitor |
| Latence API p95 | < 500ms | Application Insights |
| Chargement page (4G) | < 2s | Lighthouse > 85 |
| LCP images | < 1s | Azure CDN + WebP |
| RGPD | Données EU uniquement | Azure West Europe |
| Backup DB | RPO 24h, RTO 4h | Azure PostgreSQL automated backup |

---

## 12) Variables d'environnement (Azure Key Vault)

```bash
# PostgreSQL
ConnectionStrings__DefaultConnection  # Host=...;Database=phoenix;Username=...;Password=...

# Blob Storage
Azure__BlobStorage__ConnectionString
Azure__BlobStorage__ProductImagesContainer  # phoenix-product-images
Azure__BlobStorage__CustomerLogosContainer  # phoenix-customer-logos
Azure__BlobStorage__DocumentsContainer      # phoenix-documents
Azure__CDN__BaseUrl                          # https://cdn.phoenix-emballages.fr

# Paiement
Stripe__SecretKey
Stripe__WebhookSecret

# Emails
SendGrid__ApiKey

# Auth JWT
Jwt__Secret
Jwt__Issuer
Jwt__Audience
Jwt__ExpirationMinutes
Jwt__RefreshTokenExpirationDays

# App
App__BaseUrl
App__AllowedOrigins
```

---

## 13) Packages clés

### Phoenix.Infrastructure.csproj
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.*" />
<PackageReference Include="Azure.Storage.Blobs" Version="12.*" />
<PackageReference Include="SkiaSharp" Version="3.*" />
<PackageReference Include="SkiaSharp.NativeAssets.Linux" Version="3.*" />
<PackageReference Include="QuestPDF" Version="2024.*" />
<PackageReference Include="SendGrid" Version="9.*" />
<PackageReference Include="Stripe.net" Version="46.*" />
<PackageReference Include="Testcontainers.PostgreSQL" Version="3.*" />
```

### package.json Angular 21
```json
{
  "dependencies": {
    "@angular/core": "^21.0.0",
    "@angular/ssr": "^21.0.0",
    "primeng": "^18.0.0",
    "@primeng/themes": "^18.0.0",
    "primeicons": "^7.0.0",
    "konva": "^9.0.0"
  }
}
```

---

## 14) Seed de données

> Basé sur `PHOENIX-domain.md` — catalogue réel Phoenix 2023.

```
~100 SKUs réels répartis en deux activités.
Flags obligatoires par SKU : IsCustomizable, IsGourmetRange, IsBulkOnly,
IsEcoFriendly, IsFoodApproved, SoldByWeight, HasExpressDelivery, IsActive.
PromotionRule "5 achetés = 1 offert" → statut Active.
```