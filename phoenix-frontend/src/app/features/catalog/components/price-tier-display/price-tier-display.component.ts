import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { NgClass } from '@angular/common';
import { PriceTier, formatEur, getPriceForQuantity } from '../../models/price-tier.model';

@Component({
  selector: 'app-price-tier-display',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [],
  template: `
    <div class="price-tiers">
      <div class="price-tiers__title">Tarifs dégressifs</div>
      <table class="price-tiers__table">
        <thead>
          <tr>
            <th>Quantité</th>
            <th>Prix unitaire HT</th>
            <th>Économie</th>
          </tr>
        </thead>
        <tbody>
          @for (tier of tiers(); track tier.id; let isFirst = $first) {
            <tr [class.active]="activeTier()?.id === tier.id">
              <td>
                {{ tier.minQuantity }}
                @if (tier.maxQuantity) {
                  – {{ tier.maxQuantity }}
                } @else {
                  et +
                }
                {{ unit() }}
              </td>
              <td class="price">{{ formatEur(tier.unitPriceHT) }}</td>
              <td class="saving">
                @if (!isFirst) {
                  -{{ savingPercent(tiers()[0], tier) }}%
                }
              </td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `,
  styleUrl: './price-tier-display.component.scss'
})
export class PriceTierDisplayComponent {
  tiers           = input.required<PriceTier[]>();
  currentQuantity = input<number>(0);
  soldByWeight    = input<boolean>(false);

  readonly activeTier = computed(() =>
    getPriceForQuantity(this.tiers(), this.currentQuantity())
  );

  readonly unit = computed(() => this.soldByWeight() ? 'kg' : 'pcs');

  readonly formatEur = formatEur;

  savingPercent(baseTier: PriceTier, currentTier: PriceTier): number {
    return Math.round((1 - currentTier.unitPriceHT / baseTier.unitPriceHT) * 100);
  }
}
