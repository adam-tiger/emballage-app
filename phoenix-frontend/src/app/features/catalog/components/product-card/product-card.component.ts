import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';
import { ProductSummary } from '../../models/product-summary.model';
import { formatEur } from '../../models/price-tier.model';
import { ProductBadgeComponent } from '../product-badge/product-badge.component';
import { getPlaceholderImageUrl } from '../../../../shared/services/images.service';

@Component({
  selector: 'app-product-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, NgOptimizedImage, ProductBadgeComponent],
  template: `
    <article class="product-card" [routerLink]="['/catalogue', product().id]">

      <div class="product-card__image">
        <img
          [ngSrc]="imageSrc()"
          [alt]="product().nameFr"
          width="400"
          height="300"
          loading="lazy" />
        @if (product().isGourmetRange) {
          <span class="product-card__gourmet-overlay">★ Gamme Gourmet</span>
        }
      </div>

      <div class="product-card__body">
        <div class="product-card__family">{{ product().familyLabel }}</div>
        <h3 class="product-card__name">{{ product().nameFr }}</h3>

        <app-product-badge
          [isCustomizable]="product().isCustomizable"
          [isGourmetRange]="product().isGourmetRange"
          [isEcoFriendly]="product().isEcoFriendly"
          [isFoodApproved]="product().isFoodApproved"
          [soldByWeight]="product().soldByWeight"
          [hasExpressDelivery]="product().hasExpressDelivery"
          size="sm" />

        <div class="product-card__footer">
          <div class="product-card__price">{{ priceLabel() }}</div>
          <div class="product-card__moq">{{ moqLabel() }}</div>
        </div>
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
}
