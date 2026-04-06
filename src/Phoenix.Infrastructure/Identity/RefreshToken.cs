namespace Phoenix.Infrastructure.Identity;

/// <summary>
/// Entité EF Core représentant un refresh token persisté en base de données.
/// La valeur du token est stockée en clair ; le client reçoit le même token via Cookie HttpOnly.
/// </summary>
public sealed class RefreshToken
{
    /// <summary>Clé primaire auto-incrémentée.</summary>
    public int Id { get; set; }

    /// <summary>
    /// Valeur opaque du refresh token (64 octets encodés en Base64 Url-safe).
    /// Indexée en base pour des recherches performantes.
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>Identifiant de l'<c>ApplicationUser</c> propriétaire du token.</summary>
    public string UserId { get; set; } = default!;

    /// <summary>Date d'expiration UTC du token (7 jours après création).</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// <c>true</c> si le token a été explicitement révoqué (logout ou rotation).
    /// Un token révoqué est toujours conservé en base pour audit.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>Date de création UTC du token.</summary>
    public DateTime CreatedAt { get; set; }
}
