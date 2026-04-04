using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Phoenix.Application.Common.Behaviors;
using Phoenix.Application.Products.Commands.CreateProduct;

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
    ///   <item>Swagger / OpenAPI — avec support JWT Bearer.</item>
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

        // ── Controllers — JSON camelCase + enums en string ───────────────────
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        // ── Swagger / OpenAPI ────────────────────────────────────────────────
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "Phoenix Emballages API",
                Version     = "v1",
                Description = "API e-commerce emballages personnalisés"
            });

            // JWT Bearer dans l'interface Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name        = "Authorization",
                Type        = SecuritySchemeType.Http,
                Scheme      = "bearer",
                BearerFormat = "JWT",
                In          = ParameterLocation.Header,
                Description = "JWT Authorization. Format: Bearer {token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Commentaires XML pour enrichir la documentation Swagger
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);
        });

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
