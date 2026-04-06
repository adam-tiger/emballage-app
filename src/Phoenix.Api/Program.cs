using Microsoft.EntityFrameworkCore;
using Phoenix.Api.Extensions;
using Phoenix.Infrastructure.Extensions;
using Phoenix.Infrastructure.Persistence;
using Phoenix.Infrastructure.Persistence.DataSeed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName() // Ajout de l'enrichisseur d'environnement
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}");

    // Application Insights — uniquement si la ConnectionString est configurée
    var aiConnectionString = context.Configuration["ApplicationInsights:ConnectionString"];
    if (!string.IsNullOrWhiteSpace(aiConnectionString))
        config.WriteTo.ApplicationInsights(aiConnectionString, TelemetryConverter.Traces);
});

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddApi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// ── Application ───────────────────────────────────────────────────────────────
var app = builder.Build();

// Pipeline de middlewares
app.UsePhoenixPipeline();

// Migration automatique en développement
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PhoenixDbContext>();
    await db.Database.MigrateAsync();

    await AuthDataSeed.SeedAsync(app.Services);
}

await app.RunAsync();

// Exposition de la classe Program pour les tests d'intégration (WebApplicationFactory<Program>)
public partial class Program { }
