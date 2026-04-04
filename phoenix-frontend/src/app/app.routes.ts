import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home.page').then(m => m.HomePage)
  },
  {
    path: 'catalogue',
    loadChildren: () =>
      import('./features/catalog/catalog.routes').then(m => m.CATALOG_ROUTES)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
