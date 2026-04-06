import { Routes } from '@angular/router';

import { CustomerLayoutComponent } from './layout/customer-layout.component';

/**
 * Routes de l'espace client Phoenix.
 *
 * Toutes les routes partagent `CustomerLayoutComponent` (sidebar + main).
 * La route racine redirige vers `/espace-client/dashboard`.
 *
 * Chargé depuis app.routes.ts :
 * ```ts
 * {
 *   path: 'espace-client',
 *   canActivate: [authGuard],
 *   loadChildren: () => import('./features/customer/customer.routes')
 *     .then(m => m.CUSTOMER_ROUTES)
 * }
 * ```
 */
export const CUSTOMER_ROUTES: Routes = [
  {
    path: '',
    component: CustomerLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/dashboard/customer-dashboard.page')
            .then(m => m.CustomerDashboardPage)
      },
      {
        path: 'profil',
        loadComponent: () =>
          import('./pages/profile/customer-profile.page')
            .then(m => m.CustomerProfilePage)
      },
      {
        path: 'adresses',
        loadComponent: () =>
          import('./pages/addresses/customer-addresses.page')
            .then(m => m.CustomerAddressesPage)
      }
    ]
  }
];
