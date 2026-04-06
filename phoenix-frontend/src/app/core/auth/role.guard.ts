import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';

import { AuthService } from './auth.service';

/**
 * Guard protégeant les routes nécessitant un rôle spécifique.
 *
 * Les rôles requis sont déclarés dans `route.data['roles']` :
 * ```ts
 * {
 *   path: 'admin',
 *   canActivate: [authGuard, roleGuard],
 *   data: { roles: ['Admin', 'Employee'] }
 * }
 * ```
 *
 * Comportement :
 * - Non authentifié → `/connexion` avec `returnUrl`
 * - Authentifié sans le rôle requis → `/espace-client` (si Customer) ou `/`
 * - Aucun rôle requis dans `route.data` → accès accordé à tout utilisateur connecté
 */
export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state) => {
  const authService    = inject(AuthService);
  const router         = inject(Router);
  const requiredRoles  = route.data['roles'] as string[] | undefined;

  // ── Non authentifié ──────────────────────────────────────────────────────
  if (!authService.isAuthenticated()) {
    router.navigate(['/connexion'], {
      queryParams: { returnUrl: state.url }
    });
    return false;
  }

  // ── Aucun rôle requis → accès libre aux utilisateurs connectés ───────────
  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  // ── Vérification du rôle ─────────────────────────────────────────────────
  const user = authService.currentUser();
  if (!user) return false;

  const hasRole = requiredRoles.some(role => user.roles.includes(role));

  if (!hasRole) {
    // Redirection selon le rôle courant de l'utilisateur
    if (authService.isCustomer()) {
      router.navigate(['/espace-client']);
    } else {
      router.navigate(['/']);
    }
    return false;
  }

  return true;
};
