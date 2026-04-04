/**
 * Erreur typée retournée par le error interceptor.
 * Remplace les exceptions HTTP brutes par un objet avec codes sémantiques.
 */
export class ApiError extends Error {
  constructor(
    public readonly code: string,
    override readonly message: string,
    public readonly details?: Record<string, string[]>,
    public readonly traceId?: string,
    public readonly statusCode?: number
  ) {
    super(message);
    this.name = 'ApiError';
  }

  /** Vrai si le serveur a répondu 404 Not Found. */
  get isNotFound(): boolean {
    return this.statusCode === 404;
  }

  /** Vrai si le serveur a répondu 400 Bad Request (erreur de validation). */
  get isValidation(): boolean {
    return this.statusCode === 400;
  }

  /** Vrai si le serveur a répondu 403 Forbidden. */
  get isForbidden(): boolean {
    return this.statusCode === 403;
  }

  /** Vrai si le serveur a répondu 500 Internal Server Error. */
  get isServerError(): boolean {
    return this.statusCode === 500;
  }

  /**
   * Retourne le premier message d'erreur de validation disponible dans `details`,
   * ou `undefined` si aucun détail n'est présent.
   */
  get firstValidationError(): string | undefined {
    if (!this.details) return undefined;
    const first = Object.values(this.details)[0];
    return first?.[0];
  }
}
