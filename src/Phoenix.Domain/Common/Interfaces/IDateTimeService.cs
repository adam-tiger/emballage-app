namespace Phoenix.Domain.Common.Interfaces;

/// <summary>
/// Port (interface) du service de date/heure.
/// Permet l'injection du temps dans les handlers pour faciliter les tests unitaires.
/// L'implémentation production retourne <c>DateTime.UtcNow</c> / <c>DateOnly.FromDateTime(DateTime.UtcNow)</c>.
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    /// Date et heure courantes en UTC.
    /// Utiliser ce membre plutôt que <c>DateTime.UtcNow</c> dans la couche Application.
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Date courante en UTC sous forme de <see cref="DateOnly"/>.
    /// Utiliser ce membre pour les calculs de date pure (échéances, périodes, etc.).
    /// </summary>
    DateOnly Today { get; }
}
