using Microsoft.EntityFrameworkCore;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Infrastructure.Persistence;

namespace Phoenix.Infrastructure.Identity;

/// <summary>
/// Implémentation EF Core de <see cref="IRefreshTokenStore"/>.
/// Persiste les refresh tokens dans la table <c>refresh_tokens</c> (PostgreSQL).
/// </summary>
/// <remarks>
/// Stratégie de rotation : un token révoqué est conservé en base (non supprimé) pour audit.
/// La révocation massive utilise <c>ExecuteUpdateAsync</c> pour minimiser les aller-retours DB.
/// </remarks>
public sealed class RefreshTokenStore(PhoenixDbContext db) : IRefreshTokenStore
{
    /// <inheritdoc />
    public async Task StoreAsync(RefreshTokenData data, CancellationToken ct = default)
    {
        var entity = new RefreshToken
        {
            Token     = data.Token,
            UserId    = data.UserId,
            ExpiresAt = data.ExpiresAt,
            IsRevoked = data.IsRevoked,
            CreatedAt = data.CreatedAt
        };

        db.RefreshTokens.Add(entity);
        await db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<RefreshTokenData?> GetAsync(string token, CancellationToken ct = default)
    {
        var entity = await db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);

        if (entity is null)
            return null;

        return new RefreshTokenData(
            Token:     entity.Token,
            UserId:    entity.UserId,
            ExpiresAt: entity.ExpiresAt,
            IsRevoked: entity.IsRevoked,
            CreatedAt: entity.CreatedAt);
    }

    /// <inheritdoc />
    public async Task RevokeAsync(string token, CancellationToken ct = default)
    {
        await db.RefreshTokens
            .Where(rt => rt.Token == token)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(rt => rt.IsRevoked, true),
                ct);
    }

    /// <inheritdoc />
    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
    {
        await db.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(rt => rt.IsRevoked, true),
                ct);
    }
}
