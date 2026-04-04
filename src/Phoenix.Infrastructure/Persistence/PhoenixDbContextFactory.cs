using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Phoenix.Infrastructure.Persistence;

/// <summary>
/// Factory utilisée par les outils EF Core (migrations, scaffolding) au moment du design-time.
/// Fournit une instance de <see cref="PhoenixDbContext"/> avec une configuration minimale
/// pour permettre la création de migrations sans démarrer toute l'application.
/// </summary>
/// <remarks>
/// Cette classe n'est utilisée QUE par les commandes <c>dotnet ef</c>. 
/// En production, le DbContext est injecté normalement via le DI container.
/// </remarks>
public sealed class PhoenixDbContextFactory : IDesignTimeDbContextFactory<PhoenixDbContext>
{
    /// <summary>
    /// Crée une instance du DbContext pour les outils EF Core.
    /// Lit la chaîne de connexion depuis <c>appsettings.Development.json</c> du projet API.
    /// </summary>
    /// <param name="args">Arguments de ligne de commande (non utilisés).</param>
    /// <returns>Instance configurée du <see cref="PhoenixDbContext"/>.</returns>
    public PhoenixDbContext CreateDbContext(string[] args)
    {
        // ── 1. Charger la configuration depuis le projet API ────────────────
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Phoenix.Api"))
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        // ── 2. Configurer DbContextOptions avec Npgsql ───────────────────────
        var optionsBuilder = new DbContextOptionsBuilder<PhoenixDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsql => npgsql.MigrationsAssembly(typeof(PhoenixDbContext).Assembly.FullName));

        // ── 3. Créer un IMediator mock pour le design-time ───────────────────
        // Les événements de domaine ne sont pas dispatchés pendant les migrations
        var mediatorMock = new DesignTimeMediator();

        return new PhoenixDbContext(optionsBuilder.Options, mediatorMock);
    }

    /// <summary>
    /// Implémentation minimale de <see cref="IMediator"/> pour le design-time.
    /// Ne dispatche aucun événement — utilisée uniquement pour satisfaire le constructeur
    /// du DbContext lors de la création de migrations.
    /// </summary>
    private sealed class DesignTimeMediator : IMediator
    {
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
            IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = default)
            => AsyncEnumerable.Empty<TResponse>();

        public IAsyncEnumerable<object?> CreateStream(
            object request,
            CancellationToken cancellationToken = default)
            => AsyncEnumerable.Empty<object?>();

        public Task Publish(
            object notification,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task Publish<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification
            => Task.CompletedTask;

        public Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
            => Task.FromResult<TResponse>(default!);

        public Task Send<TRequest>(
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : IRequest
            => Task.CompletedTask;

        public Task<object?> Send(
            object request,
            CancellationToken cancellationToken = default)
            => Task.FromResult<object?>(null);
    }
}
