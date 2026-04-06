using Microsoft.AspNetCore.Identity;

namespace Phoenix.Infrastructure.Identity;

/// <summary>
/// Rôle applicatif Phoenix — étend <see cref="IdentityRole"/> avec des constantes de noms.
/// </summary>
public sealed class ApplicationRole : IdentityRole
{
    // ── Constantes de rôles ──────────────────────────────────────────────────

    /// <summary>Rôle administrateur — accès complet au back-office.</summary>
    public const string Admin = "Admin";

    /// <summary>Rôle employé Phoenix — accès limité au back-office.</summary>
    public const string Employee = "Employee";

    /// <summary>Rôle client — accès à l'espace client.</summary>
    public const string Customer = "Customer";

    // ── Constructeurs requis par Identity ────────────────────────────────────

    /// <inheritdoc />
    public ApplicationRole() : base() { }

    /// <inheritdoc />
    public ApplicationRole(string roleName) : base(roleName) { }
}
