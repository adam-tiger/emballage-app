import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiError } from '../../shared/models/api-error.model';
import { ApiErrorResponse } from '../../shared/models/api-response.model';

/**
 * Intercepteur fonctionnel qui transforme toutes les erreurs HTTP
 * en instances typées `ApiError`.
 *
 * Cas gérés :
 * - `status === 0` : réseau indisponible ou CORS
 * - Corps avec `{ code, message }` : erreur API Phoenix structurée
 * - Tous les autres cas : erreur générique
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const apiError = parseHttpError(error);
      return throwError(() => apiError);
    })
  );
};

function parseHttpError(error: HttpErrorResponse): ApiError {
  // Erreur réseau / CORS / serveur hors-ligne
  if (error.status === 0) {
    return new ApiError(
      'NETWORK_ERROR',
      'Connexion impossible. Vérifiez votre réseau.',
      undefined,
      undefined,
      0
    );
  }

  // Réponse structurée de l'API Phoenix (ApiErrorResponse)
  const body = error.error as Partial<ApiErrorResponse> | null;
  if (body && typeof body === 'object' && 'code' in body && 'message' in body) {
    return new ApiError(
      body.code!,
      body.message!,
      body.details,
      body.traceId,
      error.status
    );
  }

  // Cas de secours
  return new ApiError(
    'UNKNOWN_ERROR',
    "Une erreur inattendue s'est produite.",
    undefined,
    undefined,
    error.status
  );
}
