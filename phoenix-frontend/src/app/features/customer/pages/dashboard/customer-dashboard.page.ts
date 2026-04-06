import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { AuthService }     from '../../../../core/auth/auth.service';
import { CustomerService } from '../../services/customer.service';

/**
 * Page tableau de bord de l'espace client Phoenix.
 *
 * Affiche :
 * - Message de bienvenue personnalisé
 * - KPI cards (commandes, devis)
 * - Adresse de livraison par défaut
 * - CTA vers le catalogue
 */
@Component({
  selector: 'app-customer-dashboard-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  template: `
    <div class="dashboard">

      <!-- En-tête -->
      <div class="dashboard__header">
        <h1 class="dashboard__title">
          Bonjour, {{ authService.currentUser()?.firstName }} 👋
        </h1>
        <p class="dashboard__sub">
          Bienvenue dans votre espace client Phoenix Emballages.
        </p>
      </div>

      @if (customerService.isLoading()) {
        <div class="dashboard__loading">Chargement de votre tableau de bord...</div>
      }

      @if (customerService.dashboard()) {

        <!-- KPI Cards -->
        <div class="dashboard__kpis">

          <div class="kpi-card">
            <div class="kpi-card__icon">🛒</div>
            <div class="kpi-card__value">
              {{ customerService.dashboard()!.totalOrders }}
            </div>
            <div class="kpi-card__label">Commandes au total</div>
          </div>

          <div class="kpi-card kpi-card--accent">
            <div class="kpi-card__icon">⏳</div>
            <div class="kpi-card__value">
              {{ customerService.dashboard()!.pendingOrders }}
            </div>
            <div class="kpi-card__label">En cours de traitement</div>
          </div>

          <div class="kpi-card">
            <div class="kpi-card__icon">📋</div>
            <div class="kpi-card__value">
              {{ customerService.dashboard()!.totalQuotes }}
            </div>
            <div class="kpi-card__label">Devis demandés</div>
          </div>

          <div class="kpi-card">
            <div class="kpi-card__icon">💬</div>
            <div class="kpi-card__value">
              {{ customerService.dashboard()!.pendingQuotes }}
            </div>
            <div class="kpi-card__label">Devis en attente</div>
          </div>

        </div>

        <!-- Adresse par défaut -->
        @if (customerService.dashboard()!.defaultAddress; as addr) {
          <div class="dashboard__address">
            <h3 class="dashboard__section-title">
              📍 Adresse de livraison par défaut
            </h3>
            <div class="address-preview">
              <strong>{{ addr.label }}</strong>
              <span>{{ addr.street }}</span>
              <span>{{ addr.postalCode }} {{ addr.city }}</span>
            </div>
            <a routerLink="/espace-client/adresses" class="link-action">
              Gérer mes adresses →
            </a>
          </div>
        } @else {
          <div class="dashboard__no-address">
            <p>Vous n'avez pas encore d'adresse de livraison.</p>
            <a routerLink="/espace-client/adresses" class="btn-link">
              + Ajouter une adresse
            </a>
          </div>
        }

        <!-- CTA Catalogue -->
        <div class="dashboard__cta">
          <h3>Prêt à commander ?</h3>
          <p>
            Découvrez notre catalogue d'emballages personnalisables
            et passez votre prochaine commande.
          </p>
          <a routerLink="/catalogue" class="btn-primary">
            Voir le catalogue
          </a>
        </div>

      }

    </div>
  `,
  styleUrl: './customer-dashboard.page.scss'
})
export class CustomerDashboardPage implements OnInit {
  protected readonly customerService = inject(CustomerService);
  protected readonly authService     = inject(AuthService);

  ngOnInit(): void {
    this.customerService.loadDashboard();
  }
}
