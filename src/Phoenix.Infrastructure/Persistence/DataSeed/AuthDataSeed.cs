using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Phoenix.Application.Common.Identity;
using Phoenix.Infrastructure.Identity;

namespace Phoenix.Infrastructure.Persistence.DataSeed;

/// <summary>
/// Initialise les rôles applicatifs et l'utilisateur administrateur par défaut.
/// </summary>
/// <remarks>
/// Appelé au démarrage de l'application dans <c>Program.cs</c> via
/// <c>await AuthDataSeed.SeedAsync(app.Services);</c>.
/// Les rôles sont également seedés via <c>OnModelCreating</c> pour les migrations.
/// </remarks>
public static class AuthDataSeed
{
    /// <summary>
    /// Point d'entrée principal — seed les rôles et l'utilisateur admin.
    /// </summary>
    /// <param name="serviceProvider">Provider de services scopé.</param>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope   = serviceProvider.CreateScope();
        var roleManager   = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager   = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger        = scope.ServiceProvider.GetRequiredService<ILogger<AuthDataSeed>>();

        await SeedRolesAsync(roleManager, logger);
        await SeedAdminUserAsync(userManager, logger);
    }

    // ── Rôles ────────────────────────────────────────────────────────────────

    private static async Task SeedRolesAsync(
        RoleManager<ApplicationRole> roleManager,
        ILogger logger)
    {
        string[] roles = [ApplicationRole.Admin, ApplicationRole.Employee, ApplicationRole.Customer];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(role));
                if (result.Succeeded)
                    logger.LogInformation("Rôle créé : {Role}", role);
                else
                    logger.LogWarning("Échec de création du rôle {Role} : {Errors}",
                        role, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    // ── Utilisateur administrateur ───────────────────────────────────────────

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        const string adminEmail    = "admin@phoenix-emballages.fr";
        const string adminPassword = "Admin@Phoenix2025!";

        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName       = adminEmail,
            Email          = adminEmail,
            EmailConfirmed = true,
            FirstName      = "Admin",
            LastName       = "Phoenix",
            IsActive       = true,
            CreatedAtUtc   = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(admin, adminPassword);
        if (!createResult.Succeeded)
        {
            logger.LogWarning("Échec de création du compte admin : {Errors}",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return;
        }

        var roleResult = await userManager.AddToRoleAsync(admin, ApplicationRole.Admin);
        if (roleResult.Succeeded)
            logger.LogInformation("Compte administrateur créé : {Email}", adminEmail);
        else
            logger.LogWarning("Échec d'affectation du rôle Admin : {Errors}",
                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
    }
}
