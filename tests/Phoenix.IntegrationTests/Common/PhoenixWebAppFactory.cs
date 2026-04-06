using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phoenix.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace Phoenix.IntegrationTests.Common;

/// <summary>
/// WebApplicationFactory configurée avec Testcontainers PostgreSQL.
/// Remplace la base de données de production par un container éphémère pour chaque test session.
/// </summary>
public sealed class PhoenixWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("phoenix_test")
        .WithUsername("postgres")
        .WithPassword("postgres_test")
        .Build();

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"]   = "IntegrationTestSecretKeyThatIsLongEnoughForHmacSha256!",
                ["Jwt:Issuer"]   = "phoenix-tests",
                ["Jwt:Audience"] = "phoenix-tests",
            });
        });

        builder.ConfigureServices(services =>
        {
            // ── Remplacer la vraie DB par le container de test ──────────────
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PhoenixDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<PhoenixDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));

            // ── Authentification de test — bypass complet en testing ─────────
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme    = "TestScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "TestScheme", _ => { });
        });
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    /// <inheritdoc/>
    public new async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await base.DisposeAsync();
    }

    /// <summary>
    /// Crée un <see cref="HttpClient"/> authentifié avec le rôle spécifié.
    /// </summary>
    /// <param name="role">Rôle à injecter dans les claims (défaut : "Admin").</param>
    public HttpClient CreateAuthenticatedClient(string role = "Admin")
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("TestScheme", "test-token");
        return client;
    }
}

/// <summary>
/// Handler d'authentification de test.
/// Crée automatiquement un principal authentifié avec le rôle passé en header <c>X-Test-Role</c>.
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>Nom du header HTTP contenant le rôle de test.</summary>
    public const string RoleHeader = "X-Test-Role";

    /// <inheritdoc/>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(RoleHeader, out var roleValues))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing X-Test-Role header."));
        }

        var role = roleValues.FirstOrDefault() ?? "Employee";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email,          "test@phoenix.fr"),
            new(ClaimTypes.Name,           "Test User"),
            new(ClaimTypes.Role,           role)
        };

        var identity  = new ClaimsIdentity(claims, "TestScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket    = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
