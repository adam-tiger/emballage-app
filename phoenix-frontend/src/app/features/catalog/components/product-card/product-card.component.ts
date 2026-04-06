import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';
import { ProductSummary } from '../../models/product-summary.model';
import { formatEur } from '../../models/price-tier.model';
import { getPlaceholderImageUrl } from '../../../../shared/services/images.service';

@Component({
  selector: 'app-product-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, NgOptimizedImage],
  template: `
    <article class="product-card" [routerLink]="['/catalogue', product().id]">

      <!-- ── Badges row (top, before image) ── -->
      @if (topBadges().length > 0) {
        <div class="product-card__badges">
          @for (badge of topBadges(); track badge.label) {
            <span class="product-card__badge product-card__badge--{{ badge.type }}">
              {{ badge.label }}
            </span>
          }
        </div>
      }

      <!-- ── Image ── -->
      <div class="product-card__image">
        <img
          [ngSrc]="imageSrc()"
          [alt]="product().nameFr"
          width="400"
          height="300"
          loading="lazy" />
        <div class="product-card__image-overlay"></div>
      </div>

      <!-- ── Body ── -->
      <div class="product-card__body">
        <div class="product-card__family">{{ product().familyLabel }}</div>
        <h3 class="product-card__name">{{ product().nameFr }}</h3>

        <!-- Inline chips below name -->
        @if (inlineBadges().length > 0) {
          <div class="product-card__inline-badges">
            @for (b of inlineBadges(); track b.label) {
              <span class="product-card__inline-badge product-card__inline-badge--{{ b.type }}">
                {{ b.label }}
              </span>
            }
          </div>
        }

        <!-- Prix -->
        <div class="product-card__price">{{ priceLabel() }}</div>
        @if (moqLabel()) {
          <div class="product-card__moq">{{ moqLabel() }}</div>
        }
      </div>

      <!-- ── CTA ── -->
      <div class="product-card__cta">
        <span class="product-card__cta-btn">
          @if (product().isCustomizable) {
            🎨 Personnaliser
          } @else {
            Voir le produit →
          }
        </span>
      </div>

    </article>
  `,
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
  product = input.required<ProductSummary>();

  readonly priceLabel = computed(() => {
    const p = this.product();
    if (!p.minUnitPriceHT) return 'Prix sur devis';
    return `À partir de ${formatEur(p.minUnitPriceHT)} / pcs`;
  });

  readonly moqLabel = computed(() => {
    const p = this.product();
    if (!p.minimumOrderQuantity) return '';
    const unit = p.soldByWeight ? 'kg' : 'pcs';
    return `Dès ${p.minimumOrderQuantity} ${unit}`;
  });

  readonly imageSrc = computed(() =>
    this.product().mainImageUrl ?? getPlaceholderImageUrl()
  );

  /** Badges affichés AVANT l'image (Best Seller, Gourmet). */
  readonly topBadges = computed(() => {
    const p = this.product();
    const b: Array<{ label: string; type: string }> = [];
    if (p.isGourmetRange) b.push({ label: '★ Gamme Gourmet', type: 'gourmet' });
    return b;
  });

  /** Petits chips inline dans le body (Personnalisable, Éco, Express, Food). */
  readonly inlineBadges = computed(() => {
    const p = this.product();
    const b: Array<{ label: string; type: string }> = [];
    if (p.isCustomizable)    b.push({ label: '🎨 Personnalisable', type: 'custom' });
    if (p.isEcoFriendly)     b.push({ label: '🌿 Éco', type: 'eco' });
    if (p.hasExpressDelivery) b.push({ label: '🚀 Express', type: 'express' });
    if (p.isFoodApproved)    b.push({ label: '✓ Food', type: 'food' });
    return b;
  });
}
