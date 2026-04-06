using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Repositories;
using Phoenix.Infrastructure.Email;
using Phoenix.Infrastructure.Identity;
using Phoenix.Infrastructure.Persistence;
using Phoenix.Infrastructure.Repositories;
using Phoenix.Infrastructure.Services;
using Phoenix.Infrastructure.Storage;
using System.Text;

namespace Phoenix.Infrastructure.Extensions;

/// <summary>
/// Extension de <see cref="IServiceCollection"/> pour enregistrer tous les services de la couche Infrastructure.
/// </summary>
/// <remarks>
/// À appeler depuis <c>Program.cs</c> :
/// <code>builder.Services.AddInfrastructure(builder.Configuration);</code>
/// </remarks>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Enregistre les services Infrastructure :
    /// <list type="bullet">
    ///   <item><see cref="PhoenixDbContext"/> — EF Core 10 + Npgsql (PostgreSQL 16) avec Identity.</item>
    ///   <item>ASP.NET Core Identity — <c>UserManager</c>, <c>SignInManager</c>, <c>RoleManager</c>.</item>
    ///   <item>JWT Bearer Authentication — access token 15 min + refresh token Cookie HttpOnly.</item>
    ///   <item><see cref="IUnitOfWork"/> — délègue à <see cref="PhoenixDbContext"/>.</item>
    ///   <item><see cref="IProductRepository"/> et <see cref="ICustomerRepository"/> — repositories EF Core.</item>
    ///   <item><see cref="IJwtTokenService"/> — génération et validation de JWT.</item>
    ///   <item><see cref="IRefreshTokenStore"/> — persistance EF Core des refresh tokens.</item>
    ///   <item><see cref="IEmailService"/> — implémentation bouchon (dev) ou SendGrid (prod).</item>
    ///   <item><see cref="ICurrentUserService"/> — résolution du JWT via IHttpContextAccessor.</item>
    ///   <item><see cref="IBlobStorageService"/> — Azure Blob Storage.</item>
    ///   <item><see cref="IImageProcessingService"/> — traitement WebP via SkiaSharp 3.</item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Base de données + Identity ────────────────────────────────────────
        services.AddDbContext<PhoenixDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(PhoenixDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Politique de mot de passe
                options.Password.RequireDigit           = true;
                options.Password.RequiredLength         = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase       = true;
                options.Password.RequireLowercase       = true;

                // Verrouillage de compte
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers      = true;

                // E-mail
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<PhoenixDbContext>()
            .AddDefaultTokenProviders();

        // ── JWT Authentication ────────────────────────────────────────────────
        var jwtSecret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret est absent de la configuration.");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer           = true,
                    ValidIssuer              = configuration["Jwt:Issuer"] ?? "phoenix-api",
                    ValidateAudience         = true,
                    ValidAudience            = configuration["Jwt:Audience"] ?? "phoenix-client",
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero
                };
            });

        // ── Unité de travail ──────────────────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // ── Services Identity/Auth ────────────────────────────────────────────
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();

        // ── Services transverses ──────────────────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();

        // ── Email (Stub en dev, SendGrid en prod) ─────────────────────────────
        services.AddScoped<IEmailService, StubEmailService>();

        // ── Azure Blob Storage ────────────────────────────────────────────────
        services.Configure<BlobStorageSettings>(
            configuration.GetSection("Azure:BlobStorage"));

        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();

        // ── Image Processing (SkiaSharp) ──────────────────────────────────────
        services.AddScoped<IImageProcessingService, ImageProcessingService>();

        // ── MediatR ───────────────────────────────────────────────────────────
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(PhoenixDbContext).Assembly));

        return services;
    }
}
