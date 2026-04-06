import { Routes } from '@angular/router';

import { authGuard }  from './core/auth/auth.guard';
import { roleGuard }  from './core/auth/role.guard';

/**
 * Routes racine de l'application Phoenix Emballages.
 *
 * - Routes publiques : accueil, catalogue
 * - Routes auth : connexion/inscription/mot-de-passe-oublié (via AUTH_ROUTES + AuthLayoutComponent)
 * - Routes protégées : espace-client (authGuard), admin (authGuard + roleGuard)
 *
 * Note : guestGuard est déclaré dans AUTH_ROUTES directement sur chaque route enfant.
 */
export const routes: Routes = [
  // ── Page d'accueil ────────────────────────────────────────────────────────
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home.page').then(m => m.HomePage)
  },

  // ── Catalogue produits ────────────────────────────────────────────────────
  {
    path: 'catalogue',
    loadChildren: () =>
      import('./features/catalog/catalog.routes').then(m => m.CATALOG_ROUTES)
  },

  // ── Auth — layout split-screen partagé ────────────────────────────────────
  // connexion, inscription, mot-de-passe-oublie
  {
    path: '',
    loadChildren: () =>
      import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },

  // ── Espace client ─────────────────────────────────────────────────────────
  {
    path: 'espace-client',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/customer/customer.routes').then(m => m.CUSTOMER_ROUTES)
  },

  // ── Administration ────────────────────────────────────────────────────────
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin', 'Employee'] },
    loadChildren: () =>
      import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES)
  },

  // ── Fallback ──────────────────────────────────────────────────────────────
  {
    path: '**',
    redirectTo: ''
  }
];
