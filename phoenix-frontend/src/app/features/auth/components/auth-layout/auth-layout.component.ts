import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

/**
 * Layout split-screen réutilisé par toutes les pages d'authentification.
 *
 * Structure :
 * - Gauche (40%) : branding Phoenix — fond sombre, logo, pitch commercial
 * - Droite (60%) : `<router-outlet>` — contenu de la page (formulaire)
 *
 * Responsive : la colonne gauche est masquée en dessous de 768px.
 */
@Component({
  selector: 'app-auth-layout',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, RouterLink],
  template: `
    <div class="auth-shell">

      <!-- Côté gauche — Branding Phoenix -->
      <div class="auth-branding">
        <div class="auth-branding__inner">

          <a routerLink="/" class="auth-branding__logo">
            <span class="auth-branding__phoenix">PHOENIX</span>
            <span class="auth-branding__emballages">EMBALLAGES</span>
          </a>

          <div class="auth-branding__pitch">
            <h2 class="auth-branding__title">
              Vos emballages<br/>
              <span class="accent">à votre image.</span>
            </h2>
            <p class="auth-branding__sub">
              Leader en emballages alimentaires personnalisés
              en petite série pour la restauration.
            </p>
          </div>

          <ul class="auth-branding__args">
            <li>⚡ Livraison express 5 à 10 jours</li>
            <li>🎨 Impression recto-verso exclusif</li>
            <li>📦 Dès 50 pièces seulement</li>
            <li>✓ Agréé Contact Alimentaire</li>
          </ul>

          <div class="auth-branding__footer">
            Ils nous font confiance : Fast Foods · Boulangeries ·
            Traiteurs · Food Trucks
          </div>

        </div>
      </div>

      <!-- Côté droit — Contenu (formulaire injecté par router-outlet) -->
      <div class="auth-content">
        <div class="auth-content__inner">
          <router-outlet />
        </div>
      </div>

    </div>
  `,
  styleUrl: './auth-layout.component.scss'
})
export class AuthLayoutComponent {}
