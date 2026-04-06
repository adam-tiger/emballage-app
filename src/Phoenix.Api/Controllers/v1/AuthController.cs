using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Phoenix.Api.Models;
using Phoenix.Api.Models.Auth;
using Phoenix.Application.Auth.Commands.ForgotPassword;
using Phoenix.Application.Auth.Commands.Login;
using Phoenix.Application.Auth.Commands.Logout;
using Phoenix.Application.Auth.Commands.RefreshToken;
using Phoenix.Application.Auth.Commands.Register;
using Phoenix.Application.Auth.Commands.ResetPassword;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Auth.Queries.GetCurrentUser;

namespace Phoenix.Api.Controllers.v1;

/// <summary>
/// Contrôleur gérant l'authentification et la gestion du compte utilisateur Phoenix.
/// Implémente la stratégie JWT Bearer + Cookie HttpOnly pour le refresh token.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public sealed class AuthController(
    IMediator              mediator,
    ILogger<AuthController> logger) : ControllerBase
{
    // ── POST /api/v1/auth/register ───────────────────────────────────────────

    /// <summary>Inscrit un nouveau client sur Phoenix Emballages.</summary>
    /// <remarks>
    /// Crée simultanément un compte ASP.NET Identity et un profil client métier.
    /// Le refresh token est placé dans le Cookie HttpOnly <c>refreshToken</c> — absent du body.
    /// </remarks>
    /// <param name="model">Données d'inscription du nouveau client.</param>
    /// <response code="201">Compte créé avec succès — retourne l'access token et le profil.</response>
    /// <response code="400">Données invalides (email déjà utilisé, mot de passe trop faible, etc.).</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestModel model,
        CancellationToken ct)
    {
        var command = new RegisterCommand
        {
            Email           = model.Email,
            Password        = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            FirstName       = model.FirstName,
            LastName        = model.LastName,
            CompanyName     = model.CompanyName,
            Segment         = model.Segment
        };

        var result = await mediator.Send(command, ct);

        AppendRefreshTokenCookie(result.RefreshToken);

        logger.LogInformation(
            "Nouvelle inscription réussie : {Email}", model.Email);

        return CreatedAtAction(
            nameof(GetCurrentUser),
            null,
            new AuthResponseDto(result.AccessToken, result.ExpiresIn, result.User));
    }

    // ── POST /api/v1/auth/login ──────────────────────────────────────────────

    /// <summary>Authentifie un utilisateur existant et retourne les tokens.</summary>
    /// <remarks>
    /// Vérifie les credentials avec gestion du lockout (5 tentatives → 15 min de blocage).
    /// Le refresh token est placé dans le Cookie HttpOnly <c>refreshToken</c> — absent du body.
    /// </remarks>
    /// <param name="model">Identifiants de connexion.</param>
    /// <response code="200">Authentification réussie — retourne l'access token et le profil.</response>
    /// <response code="400">Données invalides.</response>
    /// <response code="401">Credentials incorrects ou compte verrouillé.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestModel model,
        CancellationToken ct)
    {
        var command = new LoginCommand
        {
            Email      = model.Email,
            Password   = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await mediator.Send(command, ct);

        AppendRefreshTokenCookie(result.RefreshToken);

        logger.LogInformation(
            "Connexion réussie : {Email}", model.Email);

        return Ok(new AuthResponseDto(result.AccessToken, result.ExpiresIn, result.User));
    }

    // ── POST /api/v1/auth/refresh ────────────────────────────────────────────

    /// <summary>Renouvelle l'access token en utilisant le refresh token du cookie.</summary>
    /// <remarks>
    /// Implémente la rotation des refresh tokens : l'ancien token est révoqué et un nouveau
    /// couple est émis. Le nouveau refresh token est placé dans le Cookie HttpOnly.
    /// Aucun body requis — le refresh token est lu depuis le Cookie <c>refreshToken</c>.
    /// </remarks>
    /// <response code="200">Tokens renouvelés avec succès.</response>
    /// <response code="401">Refresh token manquant, révoqué ou expiré.</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new ApiErrorResponse(
                "MISSING_REFRESH_TOKEN",
                "Refresh token manquant.",
                null,
                HttpContext.TraceIdentifier));

        var command = new RefreshTokenCommand { RefreshToken = refreshToken };
        var result  = await mediator.Send(command, ct);

        AppendRefreshTokenCookie(result.RefreshToken);

        return Ok(new AuthResponseDto(result.AccessToken, result.ExpiresIn, result.User));
    }

    // ── POST /api/v1/auth/logout ─────────────────────────────────────────────

    /// <summary>Déconnecte l'utilisateur courant et révoque ses refresh tokens.</summary>
    /// <remarks>
    /// Révoque le refresh token du cookie ainsi que tous les tokens actifs de l'utilisateur.
    /// Supprime le Cookie <c>refreshToken</c> côté client.
    /// </remarks>
    /// <response code="204">Déconnexion réussie.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refreshToken"];

        var command = new LogoutCommand { RefreshToken = refreshToken };
        await mediator.Send(command, ct);

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            Path = "/api/v1/auth"
        });

        return NoContent();
    }

    // ── POST /api/v1/auth/forgot-password ────────────────────────────────────

    /// <summary>Déclenche l'envoi d'un e-mail de réinitialisation de mot de passe.</summary>
    /// <remarks>
    /// Retourne toujours 204 — même si l'adresse e-mail n'est associée à aucun compte,
    /// afin de ne pas révéler l'existence d'un compte (protection contre l'énumération d'e-mails).
    /// </remarks>
    /// <param name="model">Adresse e-mail du compte concerné.</param>
    /// <response code="204">Demande traitée (succès ou e-mail inexistant — même réponse).</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequestModel model,
        CancellationToken ct)
    {
        await mediator.Send(new ForgotPasswordCommand { Email = model.Email }, ct);

        // Toujours 204 — ne pas révéler si l'e-mail existe (sécurité anti-énumération)
        return NoContent();
    }

    // ── POST /api/v1/auth/reset-password ─────────────────────────────────────

    /// <summary>Réinitialise le mot de passe avec le token reçu par e-mail.</summary>
    /// <param name="model">E-mail, token de reset, nouveau mot de passe et confirmation.</param>
    /// <response code="204">Mot de passe réinitialisé avec succès.</response>
    /// <response code="400">Token invalide, expiré ou mot de passe trop faible.</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequestModel model,
        CancellationToken ct)
    {
        var command = new ResetPasswordCommand
        {
            Email           = model.Email,
            Token           = model.Token,
            NewPassword     = model.NewPassword,
            ConfirmNewPassword = model.ConfirmNewPassword
        };

        await mediator.Send(command, ct);

        return NoContent();
    }

    // ── GET /api/v1/auth/me ──────────────────────────────────────────────────

    /// <summary>Retourne le profil de l'utilisateur authentifié courant.</summary>
    /// <response code="200">Profil utilisateur complet.</response>
    /// <response code="401">Token absent ou expiré.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCurrentUserQuery(), ct);
        return Ok(result);
    }

    // ── Helper privé ─────────────────────────────────────────────────────────

    /// <summary>
    /// Ajoute le refresh token dans un Cookie HttpOnly sécurisé.
    /// <c>Secure</c> est désactivé en développement (HTTP localhost) et activé en production (HTTPS).
    /// </summary>
    /// <param name="refreshToken">Valeur opaque du refresh token à persister dans le cookie.</param>
    private void AppendRefreshTokenCookie(string refreshToken)
    {
        var isDevelopment = HttpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .IsDevelopment();

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure   = !isDevelopment,
            SameSite = SameSiteMode.Strict,
            Expires  = DateTimeOffset.UtcNow.AddDays(7),
            Path     = "/api/v1/auth"
        });
    }
}
