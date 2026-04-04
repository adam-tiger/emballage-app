import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  template: `
    <div class="home-hero">
      <div class="home-hero__content">
        <span class="home-hero__label">
          Made in France · Livraison express 5 jours
        </span>
        <h1 class="home-hero__title">
          Vos emballages<br />
          <span class="accent">à votre image.</span><br />
          <span class="accent2">Dès 50 pièces.</span>
        </h1>
        <p class="home-hero__sub">
          Le seul fournisseur qui livre en 5 jours avec impression
          recto-verso et multi-couleurs — même pour les petites séries.
        </p>
        <div class="home-hero__ctas">
          <a routerLink="/catalogue" class="btn-primary">
            Voir le catalogue
          </a>
          <a routerLink="/devis" class="btn-outline">
            Devis gratuit
          </a>
        </div>
      </div>
    </div>
  `,
  styleUrl: './home.page.scss'
})
export class HomePage {}
