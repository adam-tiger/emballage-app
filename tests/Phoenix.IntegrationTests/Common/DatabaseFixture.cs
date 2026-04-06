using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Products.ValueObjects;
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

        // Seed the well-known test user used by TestAuthHandler
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var testUserId = TestAuthHandler.TestUserId.ToString();

        if (await userManager.FindByIdAsync(testUserId) is null)
        {
            // Ensure the Customer role exists
            if (!await roleManager.RoleExistsAsync(ApplicationRole.Customer))
                await roleManager.CreateAsync(new ApplicationRole(ApplicationRole.Customer));

            var testUser = new ApplicationUser
            {
                Id             = testUserId,
                UserName       = "test@phoenix.fr",
                Email          = "test@phoenix.fr",
                EmailConfirmed = true,
                FirstName      = "Test",
                LastName       = "User",
                CompanyName    = "Phoenix Test",
                Segment        = CustomerSegment.Other,
                IsActive       = true,
                CreatedAtUtc   = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(testUser, "Password1!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(testUser, ApplicationRole.Customer);
            }
        }
    }

    /// <inheritdoc/>
    public Task DisposeAsync() => Task.CompletedTask;
}
