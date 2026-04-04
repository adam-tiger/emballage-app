using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Infrastructure.Services;

/// <summary>
/// Résout le contexte de l'utilisateur authentifié depuis le JWT via <see cref="IHttpContextAccessor"/>.
/// Implémentation de <see cref="ICurrentUserService"/>.
/// </summary>
internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    /// <inheritdoc />
    public Guid? UserId
    {
        get
        {
            var sub = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    /// <inheritdoc />
    public string? UserEmail => Principal?.FindFirstValue(ClaimTypes.Email);

    /// <inheritdoc />
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public bool IsInRole(string role) => Principal?.IsInRole(role) ?? false;
}
