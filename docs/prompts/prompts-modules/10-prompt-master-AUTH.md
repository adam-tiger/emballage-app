# Prompt Master — Module Auth & Customer
# Phoenix Emballages — Sprint 2
# Prêt à coller dans Claude Code (onglet Code, Claude Desktop)

> VERSION  : 1.0 — Stack Angular 21 / .NET 10 / EF Core 10 / PostgreSQL
> MODULE   : Auth & Customer
> SPRINT   : 2 — Authentification, gestion utilisateurs, espace client
> DURÉE    : ~20h dev
> PRÉREQUIS: Module 1 (Product & Catalog) DoD validé ✅
> STATUT   : Prêt à exécuter

---

## ══════════════════════════════════════════
## PROMPT — COLLER DIRECTEMENT DANS CLAUDE CODE
## ══════════════════════════════════════════

Tu es un Lead Full Stack Angular 21 / ASP.NET Core 10 expert en Clean Architecture,
CQRS/MediatR, EF Core 10, PostgreSQL, ASP.NET Identity, JWT Bearer, et PrimeNG.

Tu génères du code **production-ready**, **complet**, **sans pseudo-code**, sans placeholder.
Chaque fichier est livré en entier. Zéro instruction "à compléter".

---

## CONTEXTE PROJET

- **Produit**    : Phoenix Emballages — E-commerce B2B d'emballages alimentaires personnalisables
- **Type**       : B2B + B2C — site public + back-office admin + espace client
- **Stack back** : .NET 10, ASP.NET Core 10, EF Core 10, **PostgreSQL** (Npgsql), MediatR,
                   FluentValidation, AutoMapper, Azure Blob Storage, SkiaSharp, QuestPDF, Serilog
- **Stack front**: Angular 21 standalone SSR, PrimeNG 18, Angular Signals, Reactive Forms
- **Hébergement**: Azure App Service Linux + Azure Database for PostgreSQL Flexible Server
                   + Azure Blob Storage + Azure CDN
- **Auth**       : ASP.NET Identity + JWT Bearer (access token) + Cookie HttpOnly (refresh token)
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
- `PHOENIX-domain.md`          → Ubiquitous language, segments client, données métier

---

## OBJECTIF DU MODULE

Générer le module **Auth & Customer** complet.
C'est le module de **sécurité et d'identité** — il protège tous les endpoints authentifiés
et fournit l'espace client personnel.

### Périmètre fonctionnel

**Authentification (public) :**
- Inscription client avec création automatique du rôle "Customer"
- Login avec émission JWT access token (15 min) + refresh token (7 jours, Cookie HttpOnly)
- Refresh token automatique côté Angular (interceptor 401 → retry)
- Logout avec révocation du refresh token
- Mot de passe oublié → email de reset (IEmailService)
- Reset de mot de passe via token

**Espace client (authentifié, rôle Customer) :**
- Dashboard client : résumé (nb commandes, nb devis en cours, dernières actions)
- Profil client : édition FirstName, LastName, CompanyName, CustomerSegment
- Gestion adresses : liste, ajout, modification

**Administration (rôles Admin + Employee) :**
- Les rôles Admin, Employee, Customer sont seedés en base
- L'endpoint GET /api/v1/auth/me retourne le profil utilisateur courant

---

## DÉCISIONS TECHNIQUES VALIDÉES — Auth

```
1. ACCESS TOKEN JWT — 15 minutes
   Stocké côté Angular en mémoire (variable signal, PAS localStorage)
   Envoyé via Authorization: Bearer <token>

2. REFRESH TOKEN — 7 jours
   Stocké dans Cookie HttpOnly, SameSite=Strict, Secure
   Table RefreshToken en DB : { Id, UserId, Token (hash), ExpiresAtUtc, CreatedAtUtc, RevokedAtUtc, ReplacedByToken }
   Rotation : chaque /auth/refresh génère un nouveau refresh token et révoque l'ancien

3. APPLICATION USER
   ApplicationUser extends IdentityUser
   Propriétés ajoutées : FirstName, LastName, CompanyName, CustomerSegment (enum),
                         CreatedAtUtc, IsActive, CustomerId (Guid? — FK vers Customer)

4. SÉPARATION IDENTITY / DOMAIN
   ApplicationUser (Infrastructure/Identity) = identité technique (login, password, roles)
   Customer (Domain) = entité métier (adresses, commandes, devis)
   Liés par CustomerId sur ApplicationUser → Customer.Id
   À l'inscription : on crée ApplicationUser + Customer en même transaction

5. RÔLES
   "Admin"    → gestion complète back-office
   "Employee" → gestion catalogue et commandes
   "Customer" → espace client, devis, commandes

6. RÔLE AUTO À L'INSCRIPTION
   RegisterCommand crée ApplicationUser + Customer + assigne rôle "Customer" automatiquement
```

---

## SPÉCIFICATIONS MÉTIER IMPÉRATIVES

### Enums domaine (réutilisés du Module 1)

```csharp
// CustomerSegment (10 valeurs) — déjà dans Domain
FastFood, BakeryPastry, JapaneseAsian, BubbleTea, RetailCommerce,
FoodTruck, Catering, ChocolateConfectionery, PizzaShop, Other
```

### Permissions endpoints

```
POST /api/v1/auth/register         → Public (non authentifié)
POST /api/v1/auth/login            → Public
POST /api/v1/auth/refresh          → Public (cookie refresh token requis)
POST /api/v1/auth/logout           → Public (cookie refresh token requis)
POST /api/v1/auth/forgot-password  → Public
POST /api/v1/auth/reset-password   → Public
GET  /api/v1/auth/me               → [Authorize] (tout utilisateur authentifié)

GET  /api/v1/customer/profile      → [Authorize(Roles = "Customer")]
PUT  /api/v1/customer/profile      → [Authorize(Roles = "Customer")]
GET  /api/v1/customer/dashboard    → [Authorize(Roles = "Customer")]
GET  /api/v1/customer/addresses    → [Authorize(Roles = "Customer")]
POST /api/v1/customer/addresses    → [Authorize(Roles = "Customer")]
PUT  /api/v1/customer/addresses/{id} → [Authorize(Roles = "Customer")]
```

---

## LIVRABLES ATTENDUS — OBLIGATOIRES

### 1. Arborescence complète

Donner l'arborescence complète backend + frontend AVANT tout code.

### 2. Domain Layer — code complet

```
Phoenix.Domain/Customers/
  Entities/
    Customer.cs                    (agrégat racine — Id, FirstName, LastName,
                                    CompanyName, CustomerSegment, Phone,
                                    CreatedAtUtc, IsActive)
    CustomerAddress.cs             (Id, CustomerId, Street, City, PostalCode,
                                    Country, IsDefault, Label)
  Events/
    CustomerRegisteredEvent.cs     (CustomerId, Email, FirstName, LastName)
  Repositories/
    ICustomerRepository.cs         (GetByIdAsync, GetByUserIdAsync, AddAsync, UpdateAsync)

Phoenix.Domain/Common/Interfaces/
  IJwtTokenService.cs              (GenerateAccessToken, GenerateRefreshToken)
  IRefreshTokenStore.cs            (StoreAsync, GetByTokenAsync, RevokeAsync)
  IEmailService.cs                 (déjà existant si Module 1 l'a prévu — sinon créer)
```

### 3. Application Layer — code complet

```
Phoenix.Application/Auth/
  Commands/
    Register/
      RegisterCommand.cs           (Email, Password, FirstName, LastName,
                                    CompanyName, CustomerSegment, Phone)
      RegisterCommandHandler.cs    (crée ApplicationUser + Customer + rôle "Customer"
                                    dans une transaction)
      RegisterCommandValidator.cs  (email valide, password 8+ chars, FirstName requis)
    Login/
      LoginCommand.cs              (Email, Password)
      LoginCommandHandler.cs       (vérifie credentials, génère access + refresh tokens,
                                    set refresh cookie HttpOnly)
      LoginCommandValidator.cs
    RefreshToken/
      RefreshTokenCommand.cs       (pas de paramètre — lit le cookie)
      RefreshTokenCommandHandler.cs (valide refresh, rotation token, nouveau access token)
    Logout/
      LogoutCommand.cs
      LogoutCommandHandler.cs      (révoque refresh token + supprime cookie)
    ForgotPassword/
      ForgotPasswordCommand.cs     (Email)
      ForgotPasswordCommandHandler.cs (génère reset token Identity, appelle IEmailService)
      ForgotPasswordCommandValidator.cs
    ResetPassword/
      ResetPasswordCommand.cs      (Email, Token, NewPassword)
      ResetPasswordCommandHandler.cs
      ResetPasswordCommandValidator.cs
  Queries/
    GetCurrentUser/
      GetCurrentUserQuery.cs
      GetCurrentUserQueryHandler.cs (retourne UserProfileDto depuis claims + DB)
  Dtos/
    AuthResponse.cs                (AccessToken, ExpiresIn, User: UserProfileDto)
    LoginRequest.cs                (Email, Password)
    RegisterRequest.cs             (Email, Password, FirstName, LastName,
                                    CompanyName, CustomerSegment, Phone)
    UserProfileDto.cs              (Id, Email, FirstName, LastName, CompanyName,
                                    CustomerSegment, Roles, CustomerId)

Phoenix.Application/Customers/
  Commands/
    UpdateCustomerProfile/
      UpdateCustomerProfileCommand.cs     (FirstName, LastName, CompanyName,
                                           CustomerSegment, Phone)
      UpdateCustomerProfileCommandHandler.cs
      UpdateCustomerProfileCommandValidator.cs
    AddCustomerAddress/
      AddCustomerAddressCommand.cs        (Street, City, PostalCode, Country,
                                           Label, IsDefault)
      AddCustomerAddressCommandHandler.cs (si IsDefault → désactiver les autres)
      AddCustomerAddressCommandValidator.cs
    UpdateCustomerAddress/
      UpdateCustomerAddressCommand.cs
      UpdateCustomerAddressCommandHandler.cs
      UpdateCustomerAddressCommandValidator.cs
  Queries/
    GetCustomerProfile/
      GetCustomerProfileQuery.cs
      GetCustomerProfileQueryHandler.cs
    GetCustomerDashboard/
      GetCustomerDashboardQuery.cs
      GetCustomerDashboardQueryHandler.cs
      CustomerDashboardDto.cs             (TotalOrders, PendingQuotes,
                                           RecentOrdersSummary)
    GetCustomerAddresses/
      GetCustomerAddressesQuery.cs
      GetCustomerAddressesQueryHandler.cs
  Dtos/
    CustomerProfileDto.cs                 (Id, FirstName, LastName, CompanyName,
                                           CustomerSegment, Phone, Email,
                                           CreatedAtUtc)
    CustomerAddressDto.cs                 (Id, Street, City, PostalCode,
                                           Country, Label, IsDefault)
    CustomerDashboardDto.cs
  Mappings/
    AuthMappingProfile.cs
    CustomerMappingProfile.cs
```

### 4. Infrastructure Layer — code complet

```
Phoenix.Infrastructure/
  Identity/
    ApplicationUser.cs                    (extends IdentityUser)
      → FirstName, LastName, CompanyName,
        CustomerSegment (enum → string en DB),
        CreatedAtUtc, IsActive, CustomerId (Guid?)

    JwtTokenService.cs                    (IJwtTokenService)
      → Génère access token JWT signé (HS256 ou RS256)
      → Claims : sub, email, roles, customerId, jti
      → Expiration : 15 min (configurable via JwtSettings)
      → Génère refresh token (crypto random base64)

    RefreshTokenStore.cs                  (IRefreshTokenStore)
      → Stocke en table RefreshToken
      → Hash du token (SHA256) — jamais en clair
      → Rotation : révoque ancien, crée nouveau
      → Purge des tokens expirés (background job ou à la demande)

    RefreshToken.cs                       (entité)
      → Id, UserId, TokenHash, ExpiresAtUtc, CreatedAtUtc,
        RevokedAtUtc, ReplacedByTokenHash

  Persistence/
    Configurations/
      ApplicationUserConfiguration.cs     (IEntityTypeConfiguration<ApplicationUser>)
      RefreshTokenConfiguration.cs        (IEntityTypeConfiguration<RefreshToken>)
      CustomerConfiguration.cs            (IEntityTypeConfiguration<Customer>)
      CustomerAddressConfiguration.cs     (IEntityTypeConfiguration<CustomerAddress>)

    PhoenixDbContext.cs
      → Ajouter : .AddIdentity<ApplicationUser, IdentityRole>()
      → DbSet<RefreshToken>
      → DbSet<Customer>
      → DbSet<CustomerAddress>
      → Ne PAS toucher aux DbSet Product du Module 1

    Migrations/
      AddAuthModule                       (EF Core 10 migration)

    DataSeed/
      RoleDataSeed.cs                     (seed "Admin", "Employee", "Customer")
      AdminUserSeed.cs                    (seed un admin par défaut pour dev)
        → Email: admin@phoenix-emballages.fr
        → Password: Admin123! (dev uniquement)
        → Rôle: Admin

  Repositories/
    CustomerRepository.cs                 (ICustomerRepository)

  Services/
    EmailService.cs                       (IEmailService — stub SMTP/SendGrid)
      → Pour ForgotPassword : envoie email avec lien reset
      → En dev : log le lien dans la console (pas de vrai envoi)
```

### 5. API Layer — code complet

```
Phoenix.Api/Controllers/v1/
  AuthController.cs
    POST /api/v1/auth/register            → RegisterCommand
      → Retourne AuthResponse (201 Created)
      → Set Cookie refresh token HttpOnly

    POST /api/v1/auth/login               → LoginCommand
      → Retourne AuthResponse (200 OK)
      → Set Cookie refresh token HttpOnly

    POST /api/v1/auth/refresh             → RefreshTokenCommand
      → Lit cookie refresh token
      → Retourne AuthResponse (200 OK)
      → Set nouveau Cookie refresh token

    POST /api/v1/auth/logout              → LogoutCommand
      → Révoque refresh token
      → Supprime cookie
      → Retourne 204 No Content

    POST /api/v1/auth/forgot-password     → ForgotPasswordCommand
      → Retourne 200 OK (toujours — ne pas révéler si email existe)

    POST /api/v1/auth/reset-password      → ResetPasswordCommand
      → Retourne 200 OK si succès

    GET  /api/v1/auth/me                  → GetCurrentUserQuery  [Authorize]
      → Retourne UserProfileDto

  CustomerController.cs
    GET  /api/v1/customer/profile         → GetCustomerProfileQuery
      [Authorize(Roles = "Customer")]

    PUT  /api/v1/customer/profile         → UpdateCustomerProfileCommand
      [Authorize(Roles = "Customer")]

    GET  /api/v1/customer/dashboard       → GetCustomerDashboardQuery
      [Authorize(Roles = "Customer")]

    GET  /api/v1/customer/addresses       → GetCustomerAddressesQuery
      [Authorize(Roles = "Customer")]

    POST /api/v1/customer/addresses       → AddCustomerAddressCommand
      [Authorize(Roles = "Customer")]

    PUT  /api/v1/customer/addresses/{id}  → UpdateCustomerAddressCommand
      [Authorize(Roles = "Customer")]
```

#### Configuration API — Program.cs ajouts

```csharp
// Ajouter dans Program.cs (ne PAS casser la config existante Module 1)

// 1. Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // V1 — pas de confirmation email
})
.AddEntityFrameworkStores<PhoenixDbContext>()
.AddDefaultTokenProviders();

// 2. JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero // Pas de tolérance sur l'expiration
    };
});

// 3. Authorization policies
builder.Services.AddAuthorization();

// 4. Cookie policy pour refresh token
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// 5. JwtSettings from appsettings.json
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
```

#### appsettings.json — section JwtSettings

```json
{
  "JwtSettings": {
    "SecretKey": "CHANGE_ME_IN_PRODUCTION_MIN_32_CHARS_LONG!!",
    "Issuer": "PhoenixEmballages",
    "Audience": "PhoenixEmballagesApp",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### 6. Frontend Angular 21 — code complet

```
src/app/core/auth/
  auth.service.ts
    → Signal-based state :
      currentUser = signal<UserProfile | null>(null)
      accessToken = signal<string | null>(null)    // EN MÉMOIRE, pas localStorage
      isAuthenticated = computed(() => !!this.currentUser())
      isAdmin = computed(() => this.currentUser()?.roles.includes('Admin') ?? false)
      isEmployee = computed(() => this.currentUser()?.roles.includes('Employee') ?? false)
      isCustomer = computed(() => this.currentUser()?.roles.includes('Customer') ?? false)

    → Méthodes :
      login(email, password): Observable<AuthResponse>
        → POST /api/v1/auth/login { withCredentials: true }
        → Stocke accessToken en signal, set currentUser
      register(request): Observable<AuthResponse>
        → POST /api/v1/auth/register { withCredentials: true }
      logout(): Observable<void>
        → POST /api/v1/auth/logout { withCredentials: true }
        → Reset signals
      refreshToken(): Observable<AuthResponse>
        → POST /api/v1/auth/refresh { withCredentials: true }
        → Met à jour accessToken signal
      loadCurrentUser(): Observable<UserProfile>
        → GET /api/v1/auth/me
        → Appelé au démarrage app (APP_INITIALIZER)
        → Si 401 → tente refreshToken() → si échec → user non connecté

  auth.guard.ts
    → canActivate : vérifie isAuthenticated()
    → Si non authentifié → redirect /auth/login avec returnUrl

  role.guard.ts
    → canActivate : vérifie rôle requis (data.roles dans route)
    → Si rôle insuffisant → redirect /unauthorized

  jwt.interceptor.ts
    → HttpInterceptorFn (functional interceptor Angular 21)
    → Ajoute Authorization: Bearer <accessToken> sur chaque requête API
    → Si 401 reçu :
      1. Tente refreshToken()
      2. Si succès → retry la requête originale avec nouveau token
      3. Si échec refresh → logout() + redirect /auth/login
    → Queue les requêtes pendant le refresh (éviter multiple refresh simultanés)
    → Exclure /auth/login, /auth/register, /auth/refresh du header Bearer

  models/
    auth.model.ts
      → AuthResponse { accessToken, expiresIn, user: UserProfile }
      → UserProfile { id, email, firstName, lastName, companyName,
                      customerSegment, roles, customerId }
      → LoginRequest { email, password }
      → RegisterRequest { email, password, firstName, lastName,
                          companyName, customerSegment, phone }

src/app/features/auth/
  pages/
    login.page.ts
      → Formulaire PrimeNG : p-inputText email, p-password password
      → Bouton "Se connecter" avec loading state
      → Lien "Mot de passe oublié ?" → /auth/forgot-password
      → Lien "Créer un compte" → /auth/register
      → Redirect vers returnUrl ou /customer/dashboard après login
      → Affichage erreur si credentials invalides (message PrimeNG p-message)
      → Labels FR : "Adresse email", "Mot de passe", "Se connecter"

    register.page.ts
      → Formulaire PrimeNG complet :
        - Email (p-inputText, validation email)
        - Mot de passe (p-password avec indicateur force)
        - Confirmer mot de passe (custom validator match)
        - Prénom (p-inputText, requis)
        - Nom (p-inputText, requis)
        - Nom de société (p-inputText, optionnel)
        - Segment client (p-dropdown : CustomerSegment enum, optionnel)
        - Téléphone (p-inputText, optionnel)
      → Bouton "Créer mon compte" avec loading state
      → Lien "Déjà un compte ? Se connecter" → /auth/login
      → Redirect vers /customer/dashboard après inscription
      → Labels FR

    forgot-password.page.ts
      → Formulaire : email uniquement
      → Bouton "Envoyer le lien de réinitialisation"
      → Message de confirmation (toujours affiché — sécurité)
      → Lien retour "← Retour à la connexion"

    reset-password.page.ts
      → Récupère email + token depuis query params
      → Formulaire : nouveau mot de passe + confirmation
      → Bouton "Réinitialiser le mot de passe"
      → Redirect vers /auth/login après succès

  auth.routes.ts
    → /auth/login        → LoginPage
    → /auth/register     → RegisterPage
    → /auth/forgot-password → ForgotPasswordPage
    → /auth/reset-password  → ResetPasswordPage

src/app/features/customer/
  pages/
    customer-dashboard.page.ts
      → Résumé client : cartes PrimeNG (p-card)
        - Nombre de commandes
        - Devis en cours
        - Dernières actions
      → Loading/error/empty states
      → Labels FR : "Mon tableau de bord", "Mes commandes", "Devis en cours"
      → NOTE : les données commandes/devis seront 0 pour l'instant
               (modules Order/Quote pas encore développés)
               → Afficher "Aucune commande" / "Aucun devis" en empty state

    customer-profile.page.ts
      → Formulaire d'édition profil PrimeNG
        - Prénom, Nom, Société, Segment, Téléphone
        - Email affiché en lecture seule
      → Bouton "Enregistrer" avec loading state
      → PrimeNG p-toast succès "Profil mis à jour"
      → Labels FR

    customer-addresses.page.ts
      → Liste des adresses (PrimeNG p-card ou p-dataView)
      → Bouton "Ajouter une adresse"
      → Dialog PrimeNG (p-dialog) pour ajout/édition
      → Badge "Par défaut" sur l'adresse principale
      → Labels FR

  services/
    customer.service.ts
      → Signal-based : profile, addresses, dashboard signals
      → loadProfile(), updateProfile(), loadDashboard()
      → loadAddresses(), addAddress(), updateAddress()

  models/
    customer.model.ts
      → CustomerProfile, CustomerAddress, CustomerDashboard interfaces

  customer.routes.ts
    → /customer/dashboard  → CustomerDashboardPage   [AuthGuard + RoleGuard("Customer")]
    → /customer/profile    → CustomerProfilePage      [AuthGuard + RoleGuard("Customer")]
    → /customer/addresses  → CustomerAddressesPage    [AuthGuard + RoleGuard("Customer")]
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
// withCredentials: true sur tous les appels auth (cookie refresh token)

// Exemple interceptor JWT Angular 21 (functional) :
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const accessToken = authService.accessToken();

  // Ne pas ajouter le token sur les endpoints auth publics
  const publicPaths = ['/auth/login', '/auth/register', '/auth/refresh'];
  if (publicPaths.some(path => req.url.includes(path))) {
    return next(req);
  }

  if (accessToken) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${accessToken}` }
    });
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Tenter refresh et retry
        return authService.refreshToken().pipe(
          switchMap(() => {
            const newToken = authService.accessToken();
            const retryReq = req.clone({
              setHeaders: { Authorization: `Bearer ${newToken}` }
            });
            return next(retryReq);
          }),
          catchError(() => {
            authService.logout();
            return throwError(() => error);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
```

### 7. Seed data — Rôles + Admin

```csharp
// RoleDataSeed.cs — seed les 3 rôles au démarrage
// Rôles : "Admin", "Employee", "Customer"
// Idempotent : ne crée pas si déjà existant (RoleManager.RoleExistsAsync)

// AdminUserSeed.cs — seed un utilisateur admin pour le dev
// Email    : admin@phoenix-emballages.fr
// Password : Admin123!
// Rôle     : Admin
// FirstName: Admin
// LastName : Phoenix
// IsActive : true
// Idempotent : ne crée pas si déjà existant (UserManager.FindByEmailAsync)
```

### 8. Tests

```
Phoenix.UnitTests/Auth/
  RegisterCommandHandlerTests.cs
    → Inscription réussie → crée ApplicationUser + Customer + rôle Customer
    → Inscription email existant → retourne erreur
    → Validation : email invalide, password trop court, FirstName vide
  LoginCommandHandlerTests.cs
    → Login réussi → retourne AuthResponse avec access + refresh tokens
    → Login credentials invalides → retourne 401
    → Login user désactivé (IsActive=false) → retourne 403
  RefreshTokenCommandHandlerTests.cs
    → Refresh valide → nouveau access + refresh tokens, ancien révoqué
    → Refresh token expiré → retourne 401
    → Refresh token déjà révoqué → retourne 401 (détection réutilisation)
  LogoutCommandHandlerTests.cs
    → Logout → refresh token révoqué, cookie supprimé

Phoenix.UnitTests/Customers/
  UpdateCustomerProfileCommandHandlerTests.cs
    → Mise à jour profil → sauvegardé correctement
    → Customer non trouvé → 404
  AddCustomerAddressCommandHandlerTests.cs
    → Ajout adresse → sauvegardée
    → Ajout adresse IsDefault → désactive les autres par défaut

Phoenix.IntegrationTests/Auth/
  AuthControllerTests.cs                   (Testcontainers.PostgreSQL)
    → POST /api/v1/auth/register → 201 + AuthResponse
    → POST /api/v1/auth/login → 200 + AuthResponse + Cookie Set-Cookie refresh
    → POST /api/v1/auth/refresh → 200 + nouveau token
    → POST /api/v1/auth/logout → 204 + Cookie supprimé
    → GET  /api/v1/auth/me sans token → 401
    → GET  /api/v1/auth/me avec token → 200 + UserProfileDto

Phoenix.IntegrationTests/Customers/
  CustomerControllerTests.cs               (Testcontainers.PostgreSQL)
    → GET /api/v1/customer/profile sans auth → 401
    → GET /api/v1/customer/profile avec rôle Employee → 403
    → GET /api/v1/customer/profile avec rôle Customer → 200
    → PUT /api/v1/customer/profile → 200
    → POST /api/v1/customer/addresses → 201
    → PUT  /api/v1/customer/addresses/{id} → 200
```

### 9. Commandes d'exécution

```bash
# ── Backend ────────────────────────────────────────────────
dotnet restore
dotnet build --configuration Release -warnaserror

# Migration PostgreSQL (EF Core 10 + Npgsql)
dotnet ef migrations add AddAuthModule \
  --project Phoenix.Infrastructure \
  --startup-project Phoenix.Api

dotnet ef database update \
  --project Phoenix.Infrastructure \
  --startup-project Phoenix.Api

# Seed rôles + admin (exécuté au démarrage via IHostedService ou Program.cs)

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
✅ Ne PAS casser le Module 1 Product — ajouter, ne pas modifier (sauf PhoenixDbContext)

Backend :
✅ Npgsql — PAS de SqlServer provider
✅ Enums stockés comme string en PostgreSQL (HasConversion<string>())
✅ ASP.NET Identity avec ApplicationUser custom
✅ JWT access token 15 min, claims : sub, email, roles, customerId, jti
✅ Refresh token en Cookie HttpOnly SameSite=Strict Secure
✅ Refresh token hashé en DB (SHA256) — JAMAIS en clair
✅ Rotation refresh token à chaque refresh
✅ ClockSkew = TimeSpan.Zero sur JWT validation
✅ POST /auth/forgot-password → toujours 200 (ne pas révéler si email existe)
✅ Serilog logs structurés dans chaque handler
✅ traceId dans toutes les réponses d'erreur
✅ Swagger annoté sur tous les endpoints
✅ Testcontainers.PostgreSQL pour les tests d'intégration

Frontend Angular 21 :
✅ Access token en signal mémoire — PAS localStorage / PAS sessionStorage
✅ withCredentials: true sur appels auth (cookie refresh)
✅ Interceptor JWT avec auto-refresh 401 + retry + queue
✅ APP_INITIALIZER : tente loadCurrentUser() au démarrage
✅ AuthGuard + RoleGuard sur routes protégées
✅ PrimeNG pour formulaires auth + espace client
✅ Site public auth pages : CSS custom Phoenix #E8552A
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
2. ARBORESCENCE (backend + frontend — montrer ce qui est AJOUTÉ par rapport au Module 1)
3. DOMAIN (Customer, CustomerAddress, events, interfaces)
4. APPLICATION AUTH (commands, queries, DTOs, validators)
5. APPLICATION CUSTOMER (commands, queries, DTOs, validators, mappings)
6. INFRASTRUCTURE (Identity, JWT, RefreshToken, EF Core config, migration, seed)
7. API (AuthController, CustomerController, Program.cs ajouts)
8. FRONTEND CORE AUTH (service, guard, interceptor, models)
9. FRONTEND PAGES AUTH (login, register, forgot-password, reset-password)
10. FRONTEND ESPACE CLIENT (dashboard, profil, adresses)
11. SEED DATA (rôles + admin)
12. TESTS (unitaires + intégration)
13. COMMANDES D'EXÉCUTION
14. CHECKLIST DOD ← remplir case par case
```

---

## CHECKLIST DOD — À VALIDER AVANT MERGE

```
Code — Identity & JWT
[ ] ApplicationUser extends IdentityUser avec FirstName, LastName, CompanyName,
    CustomerSegment, CreatedAtUtc, IsActive, CustomerId
[ ] JwtTokenService génère access token JWT 15 min avec claims corrects
[ ] RefreshToken hashé SHA256 en DB, rotation à chaque refresh
[ ] Cookie HttpOnly SameSite=Strict Secure pour refresh token
[ ] ClockSkew = TimeSpan.Zero
[ ] Rôles "Admin", "Employee", "Customer" seedés en base

Code — Domain Customer
[ ] Customer agrégat racine créé (séparé d'ApplicationUser)
[ ] CustomerAddress entité créée
[ ] CustomerRegisteredEvent créé
[ ] ICustomerRepository créé

Code — Application
[ ] RegisterCommand : crée ApplicationUser + Customer + rôle en transaction
[ ] LoginCommand : vérifie credentials + génère tokens
[ ] RefreshTokenCommand : rotation token + validation
[ ] LogoutCommand : révoque token + supprime cookie
[ ] ForgotPasswordCommand : génère reset token + appelle IEmailService
[ ] ResetPasswordCommand : reset password Identity
[ ] GetCurrentUserQuery : retourne UserProfileDto
[ ] UpdateCustomerProfileCommand + validator
[ ] AddCustomerAddressCommand + gestion IsDefault
[ ] GetCustomerProfileQuery, GetCustomerDashboardQuery

Qualité
[ ] Tests unitaires Auth handlers OK (register, login, refresh, logout)
[ ] Tests unitaires Customer handlers OK (profile, address)
[ ] Tests intégration AuthController Testcontainers.PostgreSQL OK
[ ] Tests intégration CustomerController Testcontainers.PostgreSQL OK
[ ] Lint Angular 0 erreur
[ ] Build .NET 0 warning

API/Contrats
[ ] POST /api/v1/auth/register → 201 + AuthResponse + Set-Cookie
[ ] POST /api/v1/auth/login → 200 + AuthResponse + Set-Cookie
[ ] POST /api/v1/auth/refresh → 200 + AuthResponse + nouveau Set-Cookie
[ ] POST /api/v1/auth/logout → 204 + cookie supprimé
[ ] POST /api/v1/auth/forgot-password → 200 (toujours)
[ ] POST /api/v1/auth/reset-password → 200
[ ] GET  /api/v1/auth/me → 200 UserProfileDto [Authorize]
[ ] GET/PUT /api/v1/customer/profile → [Authorize(Customer)]
[ ] GET /api/v1/customer/dashboard → [Authorize(Customer)]
[ ] GET/POST/PUT /api/v1/customer/addresses → [Authorize(Customer)]
[ ] Swagger annoté avec exemples
[ ] Codes HTTP corrects (200, 201, 204, 400, 401, 403, 404)
[ ] Contrat erreur uniforme { code, message, details, traceId }

Sécurité
[ ] Access token JAMAIS en localStorage/cookie — mémoire uniquement (signal)
[ ] Refresh token en Cookie HttpOnly SameSite=Strict Secure
[ ] Refresh token hashé en DB
[ ] Rotation refresh token empêche la réutilisation
[ ] ForgotPassword ne révèle pas si email existe
[ ] Password policy : 8+ chars, 1 chiffre minimum
[ ] Validation entrée complète (FluentValidation)
[ ] Aucun secret dans le code source (appsettings.Development.json exclus)

Base de données (PostgreSQL)
[ ] Migration EF Core 10 AddAuthModule créée
[ ] Tables Identity (AspNet*) créées
[ ] Table RefreshToken créée
[ ] Tables Customer + CustomerAddress créées
[ ] Index sur Customer.Id, CustomerAddress.CustomerId
[ ] Enums stockés en string (HasConversion<string>())
[ ] Seed rôles + admin idempotent

Frontend Angular 21
[ ] AuthService : signals currentUser, accessToken, isAuthenticated, isAdmin, isCustomer
[ ] JWT interceptor : auto-refresh 401 + retry + queue requests
[ ] APP_INITIALIZER : loadCurrentUser au démarrage
[ ] AuthGuard + RoleGuard fonctionnels
[ ] LoginPage : formulaire PrimeNG + gestion erreurs + redirect
[ ] RegisterPage : formulaire complet + validation + redirect
[ ] ForgotPasswordPage + ResetPasswordPage
[ ] CustomerDashboardPage : résumé + empty states
[ ] CustomerProfilePage : édition + save + toast succès
[ ] CustomerAddressesPage : liste + ajout/édition dialog
[ ] Composants : input() / output() / signal() / inject() / standalone
[ ] États loading / error / empty présents partout
[ ] OnPush + trackBy sur toutes les listes
[ ] CSS Phoenix sur les pages auth (#E8552A)
[ ] withCredentials: true sur appels auth
```

---

## ORDRE D'EXÉCUTION RECOMMANDÉ (8 tours)

```
Tour 1 → Arborescence + Domain Customer (entités, events, interfaces)
          + IJwtTokenService, IRefreshTokenStore interfaces dans Domain/Common
Tour 2 → Application Auth : Commands (Register, Login, Refresh, Logout,
          ForgotPassword, ResetPassword) + Query GetCurrentUser
          + DTOs + Validators
Tour 3 → Application Customer : Commands (UpdateProfile, AddAddress,
          UpdateAddress) + Queries (GetProfile, GetDashboard, GetAddresses)
          + DTOs + Validators + Mappings
Tour 4 → Infrastructure : ApplicationUser, JwtTokenService, RefreshTokenStore,
          EF Core configurations, PhoenixDbContext ajouts Identity,
          Migration AddAuthModule, Seeds (rôles + admin)
Tour 5 → API : AuthController + CustomerController + Program.cs config
          (Identity, JWT, Cookie) + Swagger
Tour 6 → Angular core/auth : AuthService (signals), jwt.interceptor.ts
          (auto-refresh 401), auth.guard.ts, role.guard.ts,
          APP_INITIALIZER, models
Tour 7 → Angular features/auth : LoginPage, RegisterPage,
          ForgotPasswordPage, ResetPasswordPage + auth.routes.ts
          Angular features/customer : DashboardPage, ProfilePage,
          AddressesPage + customer.routes.ts
Tour 8 → Tests unitaires (Auth handlers + Customer handlers)
          + Tests intégration (AuthController + CustomerController)
          + Vérification DoD complète
```

---

## VARIANTE ULTRA RAPIDE

```
En respectant les standards Phoenix (02 à 09 + PHOENIX-domain.md),
génère le module Auth & Customer complet Angular 21 + .NET 10 avec :
- Clean Architecture CQRS/MediatR, EF Core 10, PostgreSQL (Npgsql)
- ASP.NET Identity + ApplicationUser custom (FirstName, LastName, CompanyName,
  CustomerSegment, CreatedAtUtc, IsActive, CustomerId)
- JWT access token 15 min (mémoire Angular, pas localStorage)
- Refresh token 7 jours Cookie HttpOnly SameSite=Strict, hashé SHA256 en DB, rotation
- Rôles seedés : Admin, Employee, Customer (auto-assigné à l'inscription)
- Customer agrégat Domain séparé d'ApplicationUser, lié par CustomerId
- Endpoints auth : register, login, refresh, logout, forgot-password, reset-password, me
- Endpoints customer : profile CRUD, dashboard, addresses CRUD
- Interceptor Angular auto-refresh 401 + retry + queue
- APP_INITIALIZER loadCurrentUser au démarrage
- AuthGuard + RoleGuard
- Pages : Login, Register, ForgotPassword, ResetPassword
- Espace client : Dashboard, Profil, Adresses
- PrimeNG formulaires, CSS Phoenix #E8552A pages auth
- Angular 21 Signals, input()/output(), inject(), standalone, OnPush
- Migration EF Core 10 AddAuthModule + seed rôles + admin dev
- Tests xUnit + Testcontainers.PostgreSQL + Jest Angular
- États loading/error/empty, trackBy, withCredentials: true
- Swagger complet, Serilog, traceId erreurs
Code production-ready, sans pseudo-code, sans placeholder.
```

---

## MODULES SUIVANTS (après Auth & Customer validé DoD)

```
Module 3 : Customization       (configurateur 2D Konva.js + upload logo Blob)
Module 4 : Quote               (devis en ligne + PDF QuestPDF)
Module 5 : Order & Payment     (tunnel commande + Stripe webhooks)
Module 6 : Admin Dashboard     (Kanban PrimeNG + charts + KPIs)
Module 7 : Notifications       (emails SendGrid transactionnels)
```

> Règle absolue : ne pas passer au module suivant avant DoD validé.
