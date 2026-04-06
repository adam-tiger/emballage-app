import { Routes } from '@angular/router';

import { AuthLayoutComponent } from './components/auth-layout/auth-layout.component';
import { guestGuard }          from '../../core/auth/guest.guard';

/**
 * Routes du module Auth avec layout split-screen.
 *
 * Toutes les routes enfants partagent `AuthLayoutComponent` comme layout parent.
 * Le `guestGuard` redirige les utilisateurs déjà authentifiés.
 *
 * Chargé dans app.routes.ts via :
 * ```ts
 * { path: '', loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES) }
 * ```
 */
export const AUTH_ROUTES: Routes = [
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      {
        path: 'connexion',
        canActivate: [guestGuard],
        loadComponent: () =>
          import('./pages/login/login.page').then(m => m.LoginPage)
      },
      {
        path: 'inscription',
        canActivate: [guestGuard],
        loadComponent: () =>
          import('./pages/register/register.page').then(m => m.RegisterPage)
      },
      {
        path: 'mot-de-passe-oublie',
        canActivate: [guestGuard],
        loadComponent: () =>
          import('./pages/forgot-password/forgot-password.page')
            .then(m => m.ForgotPasswordPage)
      }
    ]
  }
];
