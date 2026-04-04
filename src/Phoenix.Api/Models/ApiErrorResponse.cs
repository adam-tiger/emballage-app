namespace Phoenix.Api.Models;

/// <summary>
/// Contrat d'erreur uniforme retourné par tous les endpoints de l'API Phoenix.
/// Sérialisé en JSON avec <c>JsonStringEnumConverter</c> et convention camelCase.
/// </summary>
/// <param name="Code">Code machine-readable identifiant le type d'erreur (ex : "VALIDATION_ERROR", "NOT_FOUND").</param>
/// <param name="Message">Message lisible décrivant l'erreur.</param>
/// <param name="Details">
/// Détails de validation champ par champ (null si non applicable).
/// Clé = nom du champ, valeur = liste des messages d'erreur.
/// </param>
/// <param name="TraceId">Identifiant de trace ASP.NET Core pour corrélation dans les logs.</param>
public sealed record ApiErrorResponse(
    string Code,
    string Message,
    IReadOnlyDictionary<string, string[]>? Details,
    string TraceId);
