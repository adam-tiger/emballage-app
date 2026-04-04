import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-product-badge',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="badge-list" [class.badge-list--sm]="size() === 'sm'">
      @if (isGourmetRange()) {
        <span class="badge badge--gourmet">★ Gamme Gourmet</span>
      }
      @if (isCustomizable()) {
        <span class="badge badge--custom">🎨 Personnalisable</span>
      }
      @if (hasExpressDelivery()) {
        <span class="badge badge--express">⚡ Express 24h</span>
      }
      @if (isEcoFriendly()) {
        <span class="badge badge--eco">♻️ Éco-responsable</span>
      }
      @if (isFoodApproved()) {
        <span class="badge badge--food">✓ Agréé alimentaire</span>
      }
      @if (soldByWeight()) {
        <span class="badge badge--weight">⚖️ Vendu au KG</span>
      }
    </div>
  `,
  styleUrl: './product-badge.component.scss'
})
export class ProductBadgeComponent {
  isCustomizable   = input<boolean>(false);
  isGourmetRange   = input<boolean>(false);
  isEcoFriendly    = input<boolean>(false);
  isFoodApproved   = input<boolean>(false);
  soldByWeight     = input<boolean>(false);
  hasExpressDelivery = input<boolean>(false);
  size             = input<'sm' | 'md'>('md');
}
