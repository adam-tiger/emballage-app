import { Routes } from '@angular/router';

export const CATALOG_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/catalog/catalog.page').then(m => m.CatalogPage)
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./pages/product-detail/product-detail.page').then(m => m.ProductDetailPage)
  }
];
