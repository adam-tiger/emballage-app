import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from './auth.service';

/**
 * Guard protégeant les routes nécessitant une authentification.
 *
 * - Si l'utilisateur est connecté → accès accordé.
 * - Sinon → redirection vers `/connexion` avec `returnUrl` pour reprise post-login.
 *
 * Usage dans app.routes.ts :
 * ```ts
 * canActivate: [authGuard]
 * ```
 */
export const authGuard: CanActivateFn = (_route, state) => {
  const authService = inject(AuthService);
  const router      = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  // Sauvegarder l'URL demandée pour rediriger après login
  router.navigate(['/connexion'], {
    queryParams: { returnUrl: state.url }
  });

  return false;
};
