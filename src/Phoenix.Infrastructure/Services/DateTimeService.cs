using Phoenix.Domain.Common.Interfaces;

namespace Phoenix.Infrastructure.Services;

/// <summary>
/// Implémentation production de <see cref="IDateTimeService"/>.
/// Retourne <c>DateTime.UtcNow</c> et <c>DateOnly.FromDateTime(DateTime.UtcNow)</c>.
/// Remplaceable par un stub en tests unitaires pour contrôler le temps.
/// </summary>
internal sealed class DateTimeService : IDateTimeService
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc />
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
