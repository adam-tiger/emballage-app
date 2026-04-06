import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import {
  EMPTY,
  Observable,
  catchError,
  finalize,
  of,
  tap,
  throwError,
} from 'rxjs';

import { ApiService } from '../http/api.service';
import { ApiError } from '../../shared/models/api-error.model';
import {
  AuthResponseDto,
  ForgotPasswordRequest,
  LoginRequest,
  RegisterRequest,
  ResetPasswordRequest,
} from '../../shared/models/auth.model';
import { UserProfileDto } from '../../shared/models/user-profile.model';

/**
 * Service central d'authentification Phoenix.
 *
 * Responsabilités :
 * - Gérer le state d'authentification via Angular Signals (token, profil, erreur)
 * - Persister l'access token dans sessionStorage (SSR-compatible)
 * - Déclencher le refresh de token via jwtInterceptor
 * - Exposer les computed signals isAuthenticated / isAdmin / isCustomer
 *
 * Sécurité :
 * - L'access token est stocké en sessionStorage (effacé à la fermeture de l'onglet)
 * - Le refresh token est dans le Cookie HttpOnly — jamais accessible en JS
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api    = inject(ApiService);
  private readonly router = inject(Router);

  // ── State privé ──────────────────────────────────────────────────────────

  private readonly _currentUser     = signal<UserProfileDto | null>(null);
  private readonly _accessToken     = signal<string | null>(null);
  private readonly _isLoading       = signal(false);
  private readonly _error           = signal<ApiError | null>(null);
  private readonly _tokenExpiresAt  = signal<Date | null>(null);

  // ── State public (readonly) ──────────────────────────────────────────────

  /** Profil de l'utilisateur courant, ou null si non authentifié. */
  readonly currentUser  = this._currentUser.asReadonly();

  /** JWT access token courant, ou null si non authentifié. */
  readonly accessToken  = this._accessToken.asReadonly();

  /** Vrai pendant les appels HTTP d'authentification. */
  readonly isLoading    = this._isLoading.asReadonly();

  /** Dernière erreur d'authentification, ou null. */
  readonly error        = this._error.asReadonly();

  // ── Computed signals ─────────────────────────────────────────────────────

  /** Vrai si un utilisateur est connecté avec un token valide. */
  readonly isAuthenticated = computed(
    () => this._currentUser() !== null && this._accessToken() !== null
  );

  /** Vrai si l'utilisateur courant a le rôle Admin. */
  readonly isAdmin = computed(
    () => this._currentUser()?.roles.includes('Admin') ?? false
  );

  /** Vrai si l'utilisateur courant a le rôle Employee ou Admin. */
  readonly isEmployee = computed(
    () =>
      (this._currentUser()?.roles.includes('Employee') ?? false) ||
      this.isAdmin()
  );

  /** Vrai si l'utilisateur courant a le rôle Customer. */
  readonly isCustomer = computed(
    () => this._currentUser()?.roles.includes('Customer') ?? false
  );

  /** Nom complet de l'utilisateur courant, ou chaîne vide. */
  readonly userFullName = computed(
    () => this._currentUser()?.fullName ?? ''
  );

  /** Vrai si une erreur est présente. */
  readonly hasError = computed(() => this._error() !== null);

  /**
   * Vrai si le token est expiré ou absent.
   * La marge de 30 secondes est incluse dans setAccessToken.
   */
  readonly isTokenExpired = computed(() => {
    const exp = this._tokenExpiresAt();
    if (!exp) return true;
    return new Date() >= exp;
  });

  // ── API publique ─────────────────────────────────────────────────────────

  /**
   * Authentifie l'utilisateur et met à jour le state.
   * Le refresh token est automatiquement stocké dans le Cookie HttpOnly par l'API.
   */
  login(request: LoginRequest): Observable<AuthResponseDto> {
    this._isLoading.set(true);
    this._error.set(null);

    return this.api.post<AuthResponseDto>('/api/v1/auth/login', request).pipe(
      tap(response => this._handleAuthResponse(response)),
      catchError(err => {
        this._error.set(err);
        return throwError(() => err);
      }),
      finalize(() => this._isLoading.set(false))
    );
  }

  /**
   * Inscrit un nouveau client et initialise sa session.
   * Le refresh token est automatiquement stocké dans le Cookie HttpOnly par l'API.
   */
  register(request: RegisterRequest): Observable<AuthResponseDto> {
    this._isLoading.set(true);
    this._error.set(null);

    return this.api.post<AuthResponseDto>('/api/v1/auth/register', request).pipe(
      tap(response => this._handleAuthResponse(response)),
      catchError(err => {
        this._error.set(err);
        return throwError(() => err);
      }),
      finalize(() => this._isLoading.set(false))
    );
  }

  /**
   * Déconnecte l'utilisateur côté API et efface le state local.
   * Effectue toujours la déconnexion locale même si l'appel API échoue.
   */
  logout(): Observable<void> {
    return this.api.post<void>('/api/v1/auth/logout', {}).pipe(
      tap(() => this._clearAuthState()),
      catchError(() => {
        // Déconnexion locale même si l'API est indisponible
        this._clearAuthState();
        return of(void 0);
      }),
      finalize(() => this.router.navigate(['/']))
    );
  }

  /**
   * Renouvelle l'access token via le refresh token du Cookie HttpOnly.
   * Appelé automatiquement par jwtInterceptor sur réponse 401.
   */
  refreshToken(): Observable<AuthResponseDto> {
    return this.api.post<AuthResponseDto>('/api/v1/auth/refresh', {}).pipe(
      tap(response => this._handleAuthResponse(response)),
      catchError(err => {
        this._clearAuthState();
        return throwError(() => err);
      })
    );
  }

  /**
   * Envoie le lien de réinitialisation de mot de passe.
   * Retourne toujours un succès côté serveur (anti-énumération d'e-mails).
   */
  forgotPassword(email: string): Observable<void> {
    const request: ForgotPasswordRequest = { email };
    return this.api.post<void>('/api/v1/auth/forgot-password', request);
  }

  /** Réinitialise le mot de passe avec le token reçu par e-mail. */
  resetPassword(request: ResetPasswordRequest): Observable<void> {
    return this.api.post<void>('/api/v1/auth/reset-password', request);
  }

  /**
   * Charge le profil de l'utilisateur authentifié depuis l'API.
   * Utilisé au démarrage pour restaurer la session depuis le token persisté.
   */
  loadCurrentUser(): Observable<UserProfileDto> {
    return this.api.get<UserProfileDto>('/api/v1/auth/me').pipe(
      tap(user => this._currentUser.set(user)),
      catchError(() => {
        this._clearAuthState();
        return EMPTY;
      })
    );
  }

  /**
   * Mémorise l'access token reçu du serveur avec sa date d'expiration.
   * La marge de -30 s évite les requêtes avec un token sur le point d'expirer.
   * Exposé publiquement pour que jwtInterceptor puisse le mettre à jour.
   */
  setAccessToken(token: string, expiresIn: number): void {
    this._accessToken.set(token);
    const expiresAt = new Date();
    expiresAt.setSeconds(expiresAt.getSeconds() + expiresIn - 30);
    this._tokenExpiresAt.set(expiresAt);
    this._persistToken(token, expiresAt);
  }

  /**
   * Restaure la session depuis sessionStorage au démarrage de l'application.
   * SSR-compatible : ne s'exécute que côté navigateur.
   * Appelé par APP_INITIALIZER dans app.config.ts.
   */
  initFromStorage(): void {
    if (typeof window === 'undefined') return;

    const stored    = sessionStorage.getItem('phoenix_token');
    const storedExp = sessionStorage.getItem('phoenix_token_exp');

    if (!stored || !storedExp) return;

    const expiresAt = new Date(storedExp);
    if (new Date() >= expiresAt) {
      this._clearStorage();
      return;
    }

    this._accessToken.set(stored);
    this._tokenExpiresAt.set(expiresAt);

    // Recharge le profil utilisateur depuis l'API pour valider la session
    this.loadCurrentUser().subscribe();
  }

  /** Efface l'erreur courante (ex : après fermeture d'un toast d'erreur). */
  clearError(): void {
    this._error.set(null);
  }

  // ── Privé ────────────────────────────────────────────────────────────────

  private _handleAuthResponse(response: AuthResponseDto): void {
    this._currentUser.set(response.user);
    this.setAccessToken(response.accessToken, response.expiresIn);
  }

  private _clearAuthState(): void {
    this._currentUser.set(null);
    this._accessToken.set(null);
    this._tokenExpiresAt.set(null);
    this._error.set(null);
    this._clearStorage();
  }

  private _persistToken(token: string, expiresAt: Date): void {
    if (typeof window === 'undefined') return;
    sessionStorage.setItem('phoenix_token', token);
    sessionStorage.setItem('phoenix_token_exp', expiresAt.toISOString());
  }

  private _clearStorage(): void {
    if (typeof window === 'undefined') return;
    sessionStorage.removeItem('phoenix_token');
    sessionStorage.removeItem('phoenix_token_exp');
  }
}
