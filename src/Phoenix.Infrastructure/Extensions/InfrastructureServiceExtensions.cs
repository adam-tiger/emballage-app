using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Infrastructure.Persistence;
using Phoenix.Infrastructure.Repositories;
using Phoenix.Infrastructure.Services;
using Phoenix.Infrastructure.Storage;

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
    ///   <item><see cref="PhoenixDbContext"/> — EF Core 10 + Npgsql (PostgreSQL 16).</item>
    ///   <item><see cref="IUnitOfWork"/> — délègue à <see cref="PhoenixDbContext"/>.</item>
    ///   <item><see cref="IProductRepository"/> — repository EF Core.</item>
    ///   <item><see cref="ICurrentUserService"/> — résolution du JWT via IHttpContextAccessor.</item>
    ///   <item><see cref="IDateTimeService"/> — singleton retournant <c>DateTime.UtcNow</c>.</item>
    ///   <item><see cref="IBlobStorageService"/> — Azure Blob Storage (images, logos, documents).</item>
    ///   <item><see cref="IImageProcessingService"/> — traitement WebP via SkiaSharp 3.</item>
    /// </list>
    /// </remarks>
    /// <param name="services">Collection de services ASP.NET Core.</param>
    /// <param name="configuration">Configuration applicative (connection strings, etc.).</param>
    /// <returns>La même <paramref name="services"/> pour le chaînage fluent.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Base de données ──────────────────────────────────────────────────
        services.AddDbContext<PhoenixDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(PhoenixDbContext).Assembly.FullName)));

        // ── Unité de travail ─────────────────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Repositories ─────────────────────────────────────────────────────
        services.AddScoped<IProductRepository, ProductRepository>();

        // ── Services transverses ─────────────────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();

        // ── Azure Blob Storage ───────────────────────────────────────────────
        services.Configure<BlobStorageSettings>(
            configuration.GetSection("Azure:BlobStorage"));

        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();

        // ── Image Processing (SkiaSharp) ─────────────────────────────────────
        services.AddScoped<IImageProcessingService, ImageProcessingService>();

        return services;
    }
}
