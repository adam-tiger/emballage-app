import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../../core/auth/auth.service';

/**
 * Layout de l'espace client Phoenix.
 *
 * Structure :
 * - Sidebar gauche (240px) : avatar initiales, navigation, logout
 * - Main droite : `<router-outlet>` pour les pages enfants
 */
@Component({
  selector: 'app-customer-layout',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="customer-shell">

      <!-- ── Sidebar ──────────────────────────────────────────────────── -->
      <aside class="customer-sidebar">

        <div class="customer-sidebar__header">
          <div class="customer-sidebar__avatar">
            {{ avatarInitials() }}
          </div>
          <div class="customer-sidebar__info">
            <span class="customer-sidebar__name">
              {{ authService.userFullName() }}
            </span>
            <span class="customer-sidebar__role">Espace client</span>
          </div>
        </div>

        <nav class="customer-sidebar__nav">
          <a routerLink="/espace-client/dashboard"
             routerLinkActive="active"
             class="customer-sidebar__item">
            📊 Tableau de bord
          </a>
          <a routerLink="/espace-client/commandes"
             routerLinkActive="active"
             class="customer-sidebar__item">
            🛒 Mes commandes
          </a>
          <a routerLink="/espace-client/devis"
             routerLinkActive="active"
             class="customer-sidebar__item">
            📋 Mes devis
          </a>
          <a routerLink="/espace-client/profil"
             routerLinkActive="active"
             class="customer-sidebar__item">
            👤 Mon profil
          </a>
          <a routerLink="/espace-client/adresses"
             routerLinkActive="active"
             class="customer-sidebar__item">
            📍 Mes adresses
          </a>
        </nav>

        <div class="customer-sidebar__footer">
          <a routerLink="/catalogue"
             class="customer-sidebar__item">
            🏪 Retour au catalogue
          </a>
          <button class="customer-sidebar__logout"
                  type="button"
                  (click)="logout()">
            🚪 Se déconnecter
          </button>
        </div>

      </aside>

      <!-- ── Contenu principal ─────────────────────────────────────────── -->
      <main class="customer-main">
        <router-outlet />
      </main>

    </div>
  `,
  styleUrl: './customer-layout.component.scss'
})
export class CustomerLayoutComponent {
  protected readonly authService = inject(AuthService);

  /** Initiales de l'utilisateur pour l'avatar (ex : "PM" pour Pedro Martinez). */
  protected readonly avatarInitials = computed(() => {
    const user = this.authService.currentUser();
    if (!user) return '?';
    const first = user.firstName?.[0] ?? '';
    const last  = user.lastName?.[0] ?? '';
    return `${first}${last}`.toUpperCase() || '?';
  });

  protected logout(): void {
    this.authService.logout().subscribe();
  }
}
