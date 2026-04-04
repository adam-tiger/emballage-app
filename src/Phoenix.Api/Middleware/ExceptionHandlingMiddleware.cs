using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Phoenix.Api.Models;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Products.Exceptions;
using ForbiddenException = Phoenix.Application.Common.Exceptions.ForbiddenException;

namespace Phoenix.Api.Middleware;

/// <summary>
/// Middleware global de gestion des exceptions non gérées.
/// Intercepte toutes les exceptions et les transforme en <see cref="ApiErrorResponse"/> JSON
/// avec le code HTTP approprié.
/// </summary>
/// <remarks>
/// <b>Mapping exception → HTTP :</b>
/// <list type="table">
///   <item><see cref="ValidationException"/> → 400 Bad Request (<c>VALIDATION_ERROR</c>)</item>
///   <item><see cref="NotFoundException"/> → 404 Not Found (<c>NOT_FOUND</c>)</item>
///   <item><see cref="ForbiddenException"/> → 403 Forbidden (<c>FORBIDDEN</c>)</item>
///   <item><see cref="ProductDomainException"/> → 422 Unprocessable Entity (code de l'exception)</item>
///   <item><see cref="InvalidOperationException"/> → 400 Bad Request (<c>INVALID_OPERATION</c>)</item>
///   <item><see cref="Exception"/> → 500 Internal Server Error (<c>INTERNAL_ERROR</c>)</item>
/// </list>
/// <b>Sécurité :</b> les stack traces ne sont jamais exposées en production.
/// </remarks>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IWebHostEnvironment env)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>Invoque le middleware.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(
                "Validation failure on {Path}: {Errors}",
                context.Request.Path,
                ex.Errors);

            await WriteErrorAsync(context, StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR", ex.Message, ex.Errors);
        }
        catch (NotFoundException ex)
        {
            logger.LogInformation(
                "Resource not found on {Path}: {Message}",
                context.Request.Path, ex.Message);

            await WriteErrorAsync(context, StatusCodes.Status404NotFound,
                "NOT_FOUND", ex.Message);
        }
        catch (ForbiddenException ex)
        {
            logger.LogInformation(
                "Access forbidden on {Path}: {Message}",
                context.Request.Path, ex.Message);

            await WriteErrorAsync(context, StatusCodes.Status403Forbidden,
                "FORBIDDEN", ex.Message);
        }
        catch (ProductDomainException ex)
        {
            logger.LogWarning(
                "Domain rule violated on {Path}. Code: {Code}, Message: {Message}",
                context.Request.Path, ex.Code, ex.Message);

            await WriteErrorAsync(context, StatusCodes.Status422UnprocessableEntity,
                ex.Code, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex,
                "Invalid operation on {Path}: {Message}",
                context.Request.Path, ex.Message);

            await WriteErrorAsync(context, StatusCodes.Status400BadRequest,
                "INVALID_OPERATION", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unhandled exception on {Path}",
                context.Request.Path);

            // Ne jamais exposer les détails en production
            var message = env.IsDevelopment()
                ? ex.Message
                : "An unexpected error occurred.";

            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR", message);
        }
    }

    // ── Helper ───────────────────────────────────────────────────────────────

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? details = null)
    {
        var response = new ApiErrorResponse(
            Code:    code,
            Message: message,
            Details: details,
            TraceId: context.TraceIdentifier);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions));
    }
}
