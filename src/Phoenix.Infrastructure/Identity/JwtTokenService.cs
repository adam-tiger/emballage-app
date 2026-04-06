using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Infrastructure.Identity;

/// <summary>
/// Implémentation de <see cref="IJwtTokenService"/> basée sur
/// <see cref="JwtSecurityTokenHandler"/> et HMAC-SHA256.
/// </summary>
/// <remarks>
/// Configuration attendue dans <c>appsettings.json</c> :
/// <code>
/// "Jwt": {
///   "Secret": "...",
///   "Issuer": "phoenix-api",
///   "Audience": "phoenix-client",
///   "AccessTokenExpirationMinutes": 15,
///   "RefreshTokenExpirationDays": 7
/// }
/// </code>
/// </remarks>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly string   _secret;
    private readonly string   _issuer;
    private readonly string   _audience;
    private readonly int      _accessTokenExpMinutes;
    private readonly int      _refreshTokenExpDays;

    private readonly JwtSecurityTokenHandler _handler = new();

    /// <summary>
    /// Initialise le service avec la configuration JWT injectée.
    /// </summary>
    public JwtTokenService(IConfiguration configuration)
    {
        var section = configuration.GetSection("Jwt");

        _secret                = section["Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret est absent de la configuration.");
        _issuer                = section["Issuer"]  ?? "phoenix-api";
        _audience              = section["Audience"] ?? "phoenix-client";
        _accessTokenExpMinutes = int.TryParse(section["AccessTokenExpirationMinutes"], out var m) ? m : 15;
        _refreshTokenExpDays   = int.TryParse(section["RefreshTokenExpirationDays"],   out var d) ? d : 7;
    }

    // ── IJwtTokenService ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public TokenResult GenerateTokens(UserClaims claims)
    {
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claimsList = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        claims.UserId),
            new(JwtRegisteredClaimNames.Email,      claims.Email),
            new(JwtRegisteredClaimNames.GivenName,  claims.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, claims.LastName),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString())
        };

        foreach (var role in claims.Roles)
            claimsList.Add(new Claim(ClaimTypes.Role, role));

        var accessExpires = DateTime.UtcNow.AddMinutes(_accessTokenExpMinutes);

        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claimsList,
            expires:            accessExpires,
            signingCredentials: credentials);

        var accessToken    = _handler.WriteToken(token);
        var refreshToken   = GenerateRefreshToken();
        var refreshExpires = DateTime.UtcNow.AddDays(_refreshTokenExpDays);

        return new TokenResult(accessToken, refreshToken, accessExpires, refreshExpires);
    }

    /// <inheritdoc />
    public UserClaims? ValidateAccessToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = key,
            ValidateIssuer           = true,
            ValidIssuer              = _issuer,
            ValidateAudience         = true,
            ValidAudience            = _audience,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero
        };

        try
        {
            var principal = _handler.ValidateToken(token, parameters, out _);

            var userId    = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)        ?? string.Empty;
            var email     = principal.FindFirstValue(JwtRegisteredClaimNames.Email)      ?? string.Empty;
            var firstName = principal.FindFirstValue(JwtRegisteredClaimNames.GivenName)  ?? string.Empty;
            var lastName  = principal.FindFirstValue(JwtRegisteredClaimNames.FamilyName) ?? string.Empty;
            var roles     = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

            return new UserClaims(userId, email, firstName, lastName, roles);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
