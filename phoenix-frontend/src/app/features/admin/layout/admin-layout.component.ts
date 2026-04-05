import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NgClass],
  template: `
    <div class="admin-shell">

      <!-- Sidebar -->
      <aside class="admin-sidebar">
        <div class="admin-sidebar__brand">
          <span class="admin-sidebar__logo">PHOENIX</span>
          <span class="admin-sidebar__sub">Administration</span>
        </div>

        <nav class="admin-sidebar__nav">
          <div class="admin-sidebar__section">Tableau de bord</div>
          <a routerLink="/admin/dashboard" routerLinkActive="active"
             class="admin-sidebar__item">
            📊 Vue d'ensemble
          </a>

          <div class="admin-sidebar__section">Catalogue</div>
          <a routerLink="/admin/products" routerLinkActive="active"
             class="admin-sidebar__item">
            📦 Produits
          </a>

          <div class="admin-sidebar__section">Opérations</div>
          <a routerLink="/admin/orders" routerLinkActive="active"
             class="admin-sidebar__item">
            🛒 Commandes
          </a>
          <a routerLink="/admin/quotes" routerLinkActive="active"
             class="admin-sidebar__item">
            📋 Devis
          </a>
          <a routerLink="/admin/customers" routerLinkActive="active"
             class="admin-sidebar__item">
            👥 Clients
          </a>

          <div class="admin-sidebar__section">Compte</div>
          <a routerLink="/" class="admin-sidebar__item">
            🌐 Voir le site
          </a>
        </nav>
      </aside>

      <!-- Contenu principal -->
      <main class="admin-main">
        <router-outlet />
      </main>

    </div>
  `,
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent {}
