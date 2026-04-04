namespace Phoenix.Api.Models;

/// <summary>
/// Enveloppe générique de succès pour les réponses API qui nécessitent un wrapper explicite.
/// </summary>
/// <typeparam name="T">Type de la donnée retournée.</typeparam>
/// <param name="Data">Payload de la réponse.</param>
public sealed record ApiResponse<T>(T Data);
