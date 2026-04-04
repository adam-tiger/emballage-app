using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Phoenix.Application.Common.Behaviors;
using Phoenix.Application.Products.Commands.CreateProduct;
using Phoenix.Application.Products.Mappings;

namespace Phoenix.Api.Extensions;

/// <summary>
/// Extension de <see cref="IServiceCollection"/> pour configurer tous les services de la couche API.
/// </summary>
/// <remarks>
/// À appeler depuis <c>Program.cs</c> :
/// <code>builder.Services.AddApi(builder.Configuration);</code>
/// </remarks>
public static class ApiServiceExtensions
{
    /// <summary>
    /// Enregistre les services de la couche API :
    /// <list type="bullet">
    ///   <item>MediatR — handlers + pipeline behaviors (Validation, Logging, Performance).</item>
    ///   <item>FluentValidation — scan automatique de l'assemblée Application.</item>
    ///   <item>Controllers — JSON camelCase, enums en string.</item>
    ///   <item>OpenAPI natif .NET 10 — avec support JWT Bearer via Scalar.</item>
    ///   <item>CORS — politique <c>PhoenixCors</c> basée sur <c>App:AllowedOrigins</c>.</item>
    ///   <item>JWT Authentication — paramètres depuis <c>Jwt:*</c>.</item>
    ///   <item>Authorization.</item>
    /// </list>
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <param name="configuration">Configuration applicative.</param>
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var applicationAssembly = typeof(CreateProductCommand).Assembly;

        // ── MediatR — handlers Application + pipeline behaviors ──────────────
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        // ── FluentValidation — scan automatique ──────────────────────────────
        services.AddValidatorsFromAssembly(applicationAssembly);

        // ── Mapperly Mappers ─────────────────────────────────────────────────
        services.AddSingleton<ProductMapper>();

        // ── Controllers — JSON camelCase + enums en string ───────────────────
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        // ── OpenAPI natif .NET 10 ────────────────────────────────────────────
        services.AddOpenApi();

        // ── CORS ─────────────────────────────────────────────────────────────
        services.AddCors(options =>
        {
            options.AddPolicy("PhoenixCors", policy =>
            {
                var allowedOrigins = configuration
                    .GetSection("App:AllowedOrigins")
                    .Get<string[]>() ?? ["http://localhost:4200"];

                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // ── JWT Authentication ────────────────────────────────────────────────
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = configuration["Jwt:Issuer"],
                    ValidAudience            = configuration["Jwt:Audience"],
                    IssuerSigningKey         = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                };
            });

        services.AddAuthorization();

        return services;
    }
}
