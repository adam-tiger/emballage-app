import { Routes } from '@angular/router';

import { authGuard }  from './core/auth/auth.guard';
import { guestGuard } from './core/auth/guest.guard';
import { roleGuard }  from './core/auth/role.guard';

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

  // ── Auth — routes réservées aux non-connectés ─────────────────────────────
  {
    path: 'connexion',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/pages/login/login.page').then(m => m.LoginPage)
  },
  {
    path: 'inscription',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/pages/register/register.page').then(m => m.RegisterPage)
  },
  {
    path: 'mot-de-passe-oublie',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/pages/forgot-password/forgot-password.page')
        .then(m => m.ForgotPasswordPage)
  },

  // ── Espace client ─────────────────────────────────────────────────────────
  {
    path: 'espace-client',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Customer', 'Admin', 'Employee'] },
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
