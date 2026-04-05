import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  template: `
    <!-- ── HERO ────────────────────────────────────────────────────────────── -->
    <section class="hero">
      <div class="hero__bg-glow"></div>
      <div class="hero__container">
        <div class="hero__content">
          <div class="hero__eyebrow">
            <span class="hero__badge">⚡ Livraison express 5 jours ouvrés</span>
          </div>
          <h1 class="hero__title">
            Des emballages<br />
            <span class="gradient-text">à votre image.</span><br />
            <span class="hero__title-accent">Dès 50 pièces.</span>
          </h1>
          <p class="hero__sub">
            Le seul fournisseur français à livrer en 5 jours avec impression
            recto-verso et multi-couleurs — même pour les petites séries.
            Zéro compromis sur la qualité.
          </p>
          <div class="hero__ctas">
            <a routerLink="/catalogue" class="btn-primary btn-primary--large">
              Parcourir le catalogue
              <span class="btn-arrow">→</span>
            </a>
            <a routerLink="/devis" class="btn-ghost btn-ghost--large">
              Devis gratuit en 24h
            </a>
          </div>
          <div class="hero__proof">
            <span class="hero__proof-item">
              <span class="hero__proof-icon">✓</span> Sans engagement
            </span>
            <span class="hero__proof-item">
              <span class="hero__proof-icon">✓</span> Échantillon gratuit
            </span>
            <span class="hero__proof-item">
              <span class="hero__proof-icon">✓</span> +2 000 références
            </span>
          </div>
        </div>
        <div class="hero__visual">
          <div class="hero__card-float hero__card-float--1">
            <span class="hero__card-icon">📦</span>
            <div>
              <div class="hero__card-label">Dernière commande</div>
              <div class="hero__card-value">Sacs kraft — 500 pcs</div>
            </div>
            <span class="hero__card-status">Livré ✓</span>
          </div>
          <div class="hero__card-float hero__card-float--2">
            <span class="hero__card-icon">⭐</span>
            <div>
              <div class="hero__card-value">4,9 / 5</div>
              <div class="hero__card-label">1 247 avis clients</div>
            </div>
          </div>
          <div class="hero__image-wrap">
            <div class="hero__image-placeholder">
              <span class="hero__image-icon">🏭</span>
              <p>Production Made in France</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── TRUST SIGNALS ────────────────────────────────────────────────────── -->
    <section class="trust">
      <div class="trust__container">
        <div class="trust__item">
          <span class="trust__icon">🚀</span>
          <div class="trust__text">
            <strong>Livraison express</strong>
            <span>5 jours ouvrés garantis</span>
          </div>
        </div>
        <div class="trust__divider"></div>
        <div class="trust__item">
          <span class="trust__icon">🎨</span>
          <div class="trust__text">
            <strong>Impression premium</strong>
            <span>Recto-verso, multi-couleurs</span>
          </div>
        </div>
        <div class="trust__divider"></div>
        <div class="trust__item">
          <span class="trust__icon">🌿</span>
          <div class="trust__text">
            <strong>Éco-responsable</strong>
            <span>Matières certifiées FSC</span>
          </div>
        </div>
        <div class="trust__divider"></div>
        <div class="trust__item">
          <span class="trust__icon">🏆</span>
          <div class="trust__text">
            <strong>Made in France</strong>
            <span>Production locale depuis 1987</span>
          </div>
        </div>
      </div>
    </section>

    <!-- ── BEST-SELLERS ─────────────────────────────────────────────────────── -->
    <section class="bestsellers">
      <div class="bestsellers__container">
        <div class="section-header">
          <div class="section-header__eyebrow">Nos best-sellers</div>
          <h2 class="section-header__title">
            Les emballages <span class="gradient-text">préférés</span><br />
            de nos clients
          </h2>
          <p class="section-header__sub">
            Sélectionnés pour leur rapport qualité-prix exceptionnel
            et leur polyvalence métier.
          </p>
        </div>
        <div class="bestsellers__grid">
          @for (item of bestSellers; track item.id) {
            <a routerLink="/catalogue" class="product-teaser">
              <div class="product-teaser__image">
                <span class="product-teaser__emoji">{{ item.emoji }}</span>
                @if (item.badge) {
                  <span class="product-teaser__badge">{{ item.badge }}</span>
                }
              </div>
              <div class="product-teaser__body">
                <div class="product-teaser__category">{{ item.category }}</div>
                <h3 class="product-teaser__name">{{ item.name }}</h3>
                <div class="product-teaser__price">{{ item.price }}</div>
                <div class="product-teaser__moq">{{ item.moq }}</div>
              </div>
            </a>
          }
        </div>
        <div class="bestsellers__cta">
          <a routerLink="/catalogue" class="btn-outline">
            Voir tous les produits →
          </a>
        </div>
      </div>
    </section>

    <!-- ── HOW IT WORKS ─────────────────────────────────────────────────────── -->
    <section class="how-it-works">
      <div class="how-it-works__container">
        <div class="section-header section-header--center">
          <div class="section-header__eyebrow">Comment ça marche</div>
          <h2 class="section-header__title">
            Commandez en <span class="gradient-text">3 étapes</span>
          </h2>
          <p class="section-header__sub">
            De la personnalisation à la livraison, nous gérons tout.
          </p>
        </div>
        <div class="steps">
          @for (step of steps; track step.number) {
            <div class="step">
              <div class="step__number">{{ step.number }}</div>
              <div class="step__icon">{{ step.icon }}</div>
              <h3 class="step__title">{{ step.title }}</h3>
              <p class="step__desc">{{ step.desc }}</p>
            </div>
          }
        </div>
      </div>
    </section>

    <!-- ── TESTIMONIALS ─────────────────────────────────────────────────────── -->
    <section class="testimonials">
      <div class="testimonials__container">
        <div class="section-header section-header--center">
          <div class="section-header__eyebrow">Ils nous font confiance</div>
          <h2 class="section-header__title">
            +3 500 professionnels<br />
            <span class="gradient-text">nous recommandent</span>
          </h2>
        </div>
        <div class="testimonials__grid">
          @for (t of testimonials; track t.id) {
            <div class="testimonial">
              <div class="testimonial__stars">★★★★★</div>
              <p class="testimonial__text">"{{ t.text }}"</p>
              <div class="testimonial__author">
                <div class="testimonial__avatar">{{ t.initials }}</div>
                <div>
                  <div class="testimonial__name">{{ t.name }}</div>
                  <div class="testimonial__role">{{ t.role }}</div>
                </div>
              </div>
            </div>
          }
        </div>
      </div>
    </section>

    <!-- ── FINAL CTA ────────────────────────────────────────────────────────── -->
    <section class="final-cta">
      <div class="final-cta__container">
        <div class="final-cta__card">
          <div class="final-cta__glow"></div>
          <h2 class="final-cta__title">
            Prêt à sublimer<br />
            <span class="gradient-text">vos emballages ?</span>
          </h2>
          <p class="final-cta__sub">
            Recevez votre devis personnalisé sous 24h. Aucun engagement requis.
          </p>
          <div class="final-cta__actions">
            <a routerLink="/catalogue" class="btn-primary btn-primary--large">
              Voir le catalogue
            </a>
            <a routerLink="/devis" class="btn-ghost btn-ghost--large">
              Devis gratuit
            </a>
          </div>
          <div class="final-cta__reassurance">
            <span>🔒 Paiement sécurisé</span>
            <span>🇫🇷 Stock France</span>
            <span>📞 SAV dédié</span>
          </div>
        </div>
      </div>
    </section>
  `,
  styleUrl: './home.page.scss'
})
export class HomePage {

  readonly bestSellers = [
    {
      id: 1,
      emoji: '🛍️',
      category: 'Sacs papier',
      name: 'Sac Kraft Brun Standard',
      price: 'À partir de 0,08 € / pcs',
      moq: 'Dès 250 pièces',
      badge: 'Best-seller'
    },
    {
      id: 2,
      emoji: '📦',
      category: 'Boîtes',
      name: 'Boîte Luxe Magnétique',
      price: 'À partir de 0,45 € / pcs',
      moq: 'Dès 100 pièces',
      badge: '★ Gourmet'
    },
    {
      id: 3,
      emoji: '🎁',
      category: 'Pochettes',
      name: 'Pochette Cadeau Premium',
      price: 'À partir de 0,12 € / pcs',
      moq: 'Dès 500 pièces',
      badge: null
    },
    {
      id: 4,
      emoji: '🌿',
      category: 'Éco-responsable',
      name: 'Sac Coton Bio Naturel',
      price: 'À partir de 0,95 € / pcs',
      moq: 'Dès 50 pièces',
      badge: 'Éco ♻'
    }
  ];

  readonly steps = [
    {
      number: '01',
      icon: '🎨',
      title: 'Choisissez & personnalisez',
      desc: 'Parcourez notre catalogue de +2 000 références. Ajoutez votre logo, vos couleurs, votre message.'
    },
    {
      number: '02',
      icon: '📋',
      title: 'Recevez votre devis',
      desc: 'Sous 24h, notre équipe vous envoie un devis détaillé avec BAT numérique offert.'
    },
    {
      number: '03',
      icon: '🚀',
      title: 'Livraison en 5 jours',
      desc: 'Votre commande est produite et expédiée en France. Livraison express partout en Europe.'
    }
  ];

  readonly testimonials = [
    {
      id: 1,
      text: 'Qualité irréprochable et délais tenus. Nos sacs kraft personnalisés ont été livrés en 4 jours. Je recommande vivement !',
      name: 'Sophie M.',
      initials: 'SM',
      role: 'Créatrice, Épicerie fine à Lyon'
    },
    {
      id: 2,
      text: 'Premier fournisseur à accepter nos petites séries sans surcoût. Le SAV est réactif et les impressions sont superbes.',
      name: 'Thomas B.',
      initials: 'TB',
      role: 'Fondateur, Chocolaterie artisanale'
    },
    {
      id: 3,
      text: 'Nous commandons depuis 3 ans. La gamme éco-responsable est parfaite pour notre image de marque engagée.',
      name: 'Amira L.',
      initials: 'AL',
      role: 'Directrice achats, Réseau bio national'
    }
  ];
}
