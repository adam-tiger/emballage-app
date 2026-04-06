import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from './auth.service';

/**
 * Guard pour les routes réservées aux utilisateurs non connectés
 * (login, inscription, mot de passe oublié).
 *
 * - Non authentifié → accès accordé.
 * - Admin ou Employee connecté → redirection vers `/admin`.
 * - Customer connecté → redirection vers `/espace-client`.
 *
 * Usage dans app.routes.ts :
 * ```ts
 * { path: 'connexion', canActivate: [guestGuard], loadComponent: ... }
 * ```
 */
export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router      = inject(Router);

  if (!authService.isAuthenticated()) {
    return true;
  }

  // Redirection intelligente selon le rôle
  if (authService.isAdmin() || authService.isEmployee()) {
    router.navigate(['/admin']);
  } else {
    router.navigate(['/espace-client']);
  }

  return false;
};
