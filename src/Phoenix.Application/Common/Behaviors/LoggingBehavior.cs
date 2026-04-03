using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Phoenix.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs request handling with structured logging.
/// Logs request start, completion time, and any exceptions.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();

        logger.LogInformation(
            "Handling {RequestName} {@Request}",
            requestName,
            request);

        try
        {
            var response = await next(cancellationToken);
            sw.Stop();

            logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs}ms",
                requestName,
                sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();

            logger.LogError(
                ex,
                "Error handling {RequestName}: {Error}",
                requestName,
                ex.Message);

            throw;
        }
    }
}
