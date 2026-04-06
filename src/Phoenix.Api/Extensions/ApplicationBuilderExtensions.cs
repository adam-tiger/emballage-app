using Microsoft.AspNetCore.OpenApi;
using Phoenix.Api.Middleware;
using Scalar.AspNetCore;

namespace Phoenix.Api.Extensions;

/// <summary>
/// Extensions de <see cref="WebApplication"/> pour configurer le pipeline de middlewares Phoenix.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configure le pipeline de middlewares ASP.NET Core dans l'ordre requis :
    /// <list type="ordered">
    ///   <item><see cref="CorrelationIdMiddleware"/> — propagation du X-Correlation-Id.</item>
    ///   <item><see cref="ExceptionHandlingMiddleware"/> — gestion globale des exceptions.</item>
    ///   <item>OpenAPI endpoint + Scalar UI (environnement Development uniquement).</item>
    ///   <item>HTTPS Redirection.</item>
    ///   <item>CORS (<c>PhoenixCors</c>).</item>
    ///   <item>Authentication.</item>
    ///   <item>Authorization.</item>
    ///   <item>MapControllers.</item>
    /// </list>
    /// </summary>
    /// <param name="app">L'application Web à configurer.</param>
    /// <returns>La même instance pour le chaînage fluent.</returns>
    public static WebApplication UsePhoenixPipeline(this WebApplication app)
    {
        // CorrelationId AVANT ExceptionHandling pour que le header soit présent
        // même dans les réponses d'erreur.
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            // OpenAPI endpoint natif .NET 10
            app.MapOpenApi();

            // Scalar UI — remplace Swagger UI
            app.MapScalarApiReference(options =>
            {
                options.Title = "Phoenix Emballages API";
                options.Theme = ScalarTheme.DeepSpace;
                options.DefaultHttpClient =
                    new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("PhoenixCors");
        app.UseCookiePolicy();    // AVANT UseAuthentication
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
