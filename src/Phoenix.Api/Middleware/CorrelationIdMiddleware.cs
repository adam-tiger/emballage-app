using Serilog.Context;

namespace Phoenix.Api.Middleware;

/// <summary>
/// Middleware qui propage ou génère un identifiant de corrélation pour chaque requête HTTP.
/// Enrichit le <c>LogContext</c> Serilog avec la propriété <c>CorrelationId</c>.
/// </summary>
/// <remarks>
/// <b>Comportement :</b>
/// <list type="bullet">
///   <item>Lit le header <c>X-Correlation-Id</c> si présent dans la requête.</item>
///   <item>Génère un nouveau <c>Guid</c> sinon.</item>
///   <item>Stocke l'identifiant dans <c>HttpContext.Items["CorrelationId"]</c>.</item>
///   <item>Ajoute le header <c>X-Correlation-Id</c> dans la réponse.</item>
///   <item>Enrichit le <c>LogContext</c> Serilog pour tous les logs de la requête.</item>
/// </list>
/// <b>Ordre dans le pipeline :</b> doit être le premier middleware enregistré,
/// avant <c>ExceptionHandlingMiddleware</c>.
/// </remarks>
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private const string CorrelationIdKey    = "CorrelationId";

    /// <summary>Invoque le middleware.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // ── 1. Lire ou générer le CorrelationId ──────────────────────────────
        var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existing)
            && !string.IsNullOrWhiteSpace(existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString();

        // ── 2. Stocker dans HttpContext pour accès downstream ────────────────
        context.Items[CorrelationIdKey] = correlationId;

        // ── 3. Ajouter dans la réponse ───────────────────────────────────────
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        // ── 4. Enrichir Serilog LogContext ───────────────────────────────────
        using (LogContext.PushProperty(CorrelationIdKey, correlationId))
        {
            await next(context);
        }
    }
}
