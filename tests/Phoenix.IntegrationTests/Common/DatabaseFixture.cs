using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Phoenix.Infrastructure.Persistence;

namespace Phoenix.IntegrationTests.Common;

/// <summary>
/// Fixture de base de données partagée entre les tests d'intégration.
/// Applique les migrations EF Core sur le container PostgreSQL de test
/// et insère les données de seed.
/// </summary>
public sealed class DatabaseFixture : IAsyncLifetime
{
    private readonly PhoenixWebAppFactory _factory;

    public DatabaseFixture(PhoenixWebAppFactory factory)
    {
        _factory = factory;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PhoenixDbContext>();

        // Applique les migrations sur le container de test
        await db.Database.MigrateAsync();
    }

    /// <inheritdoc/>
    public Task DisposeAsync() => Task.CompletedTask;
}
