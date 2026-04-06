import { inject, signal } from '@angular/core';
import {
  HttpErrorResponse,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { Observable, catchError, switchMap, throwError } from 'rxjs';

import { AuthService } from '../auth/auth.service';
import { ApiError } from '../../shared/models/api-error.model';
import { ApiErrorResponse } from '../../shared/models/api-response.model';
import { AuthResponseDto } from '../../shared/models/auth.model';

/**
 * Intercepteur HTTP fonctionnel Angular 21.
 *
 * Responsabilités :
 * 1. **Injection du token** : ajoute le header `Authorization: Bearer <token>`
 *    si un access token est disponible dans AuthService.
 * 2. **Parsing d'erreurs** : transforme les erreurs HTTP en instances typées `ApiError`.
 * 3. **Auto-refresh (401)** : si le serveur répond 401, tente de renouveler le token
 *    via le Cookie HttpOnly puis rejoue la requête originale.
 *
 * Note : `isRefreshing` est local à chaque invocation de l'interceptor.
 * Pour un vrai mutex évitant les refresh simultanés, utiliser un signal partagé
 * dans AuthService. Acceptable pour le MVP.
 *
 * Remplace `errorInterceptor` — déclaré AVANT `loadingInterceptor` dans app.config.ts.
 */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const isRefreshing = signal(false);

  // ── 1. Injecter le token Bearer ──────────────────────────────────────────
  const token   = authService.accessToken();
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((rawError: unknown) => {
      const error = rawError instanceof HttpErrorResponse ? rawError : null;

      // ── 2. Auto-refresh sur 401 ────────────────────────────────────────
      if (
        error &&
        error.status === 401 &&
        !isRefreshing() &&
        !req.url.includes('/auth/refresh') &&
        !req.url.includes('/auth/login')
      ) {
        isRefreshing.set(true);

        return authService.refreshToken().pipe(
          switchMap(() => {
            isRefreshing.set(false);
            // Rejouer la requête originale avec le nouveau token
            const newToken   = authService.accessToken();
            const retryReq   = newToken
              ? req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } })
              : req;
            return next(retryReq);
          }),
          catchError(refreshError => {
            isRefreshing.set(false);
            // Refresh échoué → déconnexion forcée
            authService.logout().subscribe();
            return throwError(() => parseHttpError(refreshError));
          })
        ) as ReturnType<HttpHandlerFn>;
      }

      // ── 3. Transformer l'erreur HTTP en ApiError typée ─────────────────
      if (error) {
        return throwError(() => parseHttpError(error));
      }

      return throwError(() => rawError);
    })
  );
};

/**
 * Transforme une `HttpErrorResponse` en instance typée `ApiError`.
 * Gère : erreurs réseau (status 0), réponses structurées Phoenix et cas génériques.
 */
function parseHttpError(error: HttpErrorResponse): ApiError {
  if (error.status === 0) {
    return new ApiError(
      'NETWORK_ERROR',
      'Connexion impossible. Vérifiez votre réseau.',
      undefined,
      undefined,
      0
    );
  }

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

  return new ApiError(
    'UNKNOWN_ERROR',
    "Une erreur inattendue s'est produite.",
    undefined,
    undefined,
    error.status
  );
}
