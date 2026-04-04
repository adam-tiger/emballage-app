import { Routes } from '@angular/router';
import { adminGuard } from './guards/admin.guard';
import { AdminLayoutComponent } from './layout/admin-layout.component';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [adminGuard],
    children: [
      {
        path: '',
        redirectTo: 'products',
        pathMatch: 'full'
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./products/pages/admin-product-list/admin-product-list.page')
            .then(m => m.AdminProductListPage)
      },
      {
        path: 'products/new',
        loadComponent: () =>
          import('./products/pages/admin-product-form/admin-product-form.page')
            .then(m => m.AdminProductFormPage)
      },
      {
        path: 'products/:id/edit',
        loadComponent: () =>
          import('./products/pages/admin-product-form/admin-product-form.page')
            .then(m => m.AdminProductFormPage)
      }
    ]
  }
];
