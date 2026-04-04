import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  input,
  output,
  signal
} from '@angular/core';
import { NgClass } from '@angular/common';
import { PriceTierDisplayComponent } from '../price-tier-display/price-tier-display.component';
import { ProductVariant, PRINT_SIDE_LABELS, COLOR_COUNT_LABELS } from '../../models/product-variant.model';
import { PrintSide } from '../../models/product-family.model';
import { formatEur, getPriceForQuantity } from '../../models/price-tier.model';

@Component({
  selector: 'app-variant-selector',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ PriceTierDisplayComponent],
  template: `
    <!-- Sélection variante si plusieurs -->
    @if (variants().length > 1) {
      <div class="variant-selector__variants">
        <div class="variant-selector__label">Format</div>
        <div class="variant-selector__buttons">
          @for (v of variants(); track v.id) {
            <button
              class="variant-btn"
              [class.variant-btn--active]="selectedVariantId() === v.id"
              (click)="selectVariant(v)">
              {{ v.nameFr }}
            </button>
          }
        </div>
      </div>
    }

    <!-- Infos impression -->
    @if (printInfo()) {
      <div class="variant-selector__print">
        <span class="print-tag">{{ printInfo()!.printSideLabel }}</span>
        <span class="print-tag">{{ printInfo()!.colorLabel }}</span>
        @if (printInfo()!.isDoubleSide) {
          <span class="print-tag print-tag--exclusive">✦ Exclusif Phoenix</span>
        }
      </div>
    }

    <!-- Saisie quantité -->
    @if (selectedVariant()) {
      <div class="variant-selector__quantity">
        <label class="variant-selector__label">Quantité</label>
        <input
          class="quantity-input"
          type="number"
          [min]="selectedVariant()!.minimumOrderQuantity"
          [value]="quantity()"
          (change)="onQuantityChange($event)" />
        @if (quantityError()) {
          <span class="quantity-error">{{ quantityError() }}</span>
        }
      </div>
    }

    <!-- Tableau paliers -->
    @if (selectedVariant()) {
      <app-price-tier-display
        [tiers]="selectedVariant()!.priceTiers"
        [currentQuantity]="quantity()"
        [soldByWeight]="soldByWeight()" />
    }

    <!-- Récap prix -->
    @if (totalHT() && !quantityError()) {
      <div class="variant-selector__total">
        <span class="total-label">Total HT</span>
        <span class="total-price">{{ formatEur(totalHT()!) }}</span>
      </div>
    }
  `,
  styleUrl: './variant-selector.component.scss'
})
export class VariantSelectorComponent implements OnInit {
  variants     = input.required<ProductVariant[]>();
  soldByWeight = input<boolean>(false);

  variantSelected = output<{ variant: ProductVariant; quantity: number }>();

  readonly selectedVariantId = signal<string | null>(null);
  readonly quantity          = signal<number>(0);

  readonly formatEur = formatEur;

  readonly selectedVariant = computed(() =>
    this.variants().find(v => v.id === this.selectedVariantId()) ?? null
  );

  readonly currentPrice = computed(() => {
    const v = this.selectedVariant();
    if (!v || this.quantity() === 0) return null;
    return getPriceForQuantity(v.priceTiers, this.quantity());
  });

  readonly totalHT = computed(() => {
    const tier = this.currentPrice();
    if (!tier) return null;
    return tier.unitPriceHT * this.quantity();
  });

  readonly printInfo = computed(() => {
    const v = this.selectedVariant();
    if (!v) return null;
    return {
      printSideLabel: PRINT_SIDE_LABELS[v.printSide],
      colorLabel:     COLOR_COUNT_LABELS[v.colorCount],
      coefficient:    v.printCoefficient,
      isDoubleSide:   v.printSide === PrintSide.DoubleSide
    };
  });

  readonly quantityError = computed(() => {
    const v = this.selectedVariant();
    if (!v || this.quantity() === 0) return null;
    if (this.quantity() < v.minimumOrderQuantity) {
      const unit = this.soldByWeight() ? 'kg' : 'pcs';
      return `Minimum ${v.minimumOrderQuantity} ${unit}`;
    }
    return null;
  });

  ngOnInit(): void {
    const first = this.variants()[0];
    if (!first) return;
    if (this.variants().length === 1) {
      this.selectedVariantId.set(first.id);
    }
    this.quantity.set(first.minimumOrderQuantity);
  }

  selectVariant(v: ProductVariant): void {
    this.selectedVariantId.set(v.id);
    this.quantity.set(v.minimumOrderQuantity);
    this.emitIfValid();
  }

  onQuantityChange(event: Event): void {
    const value = parseInt((event.target as HTMLInputElement).value, 10);
    if (!isNaN(value) && value > 0) {
      this.quantity.set(value);
      this.emitIfValid();
    }
  }

  private emitIfValid(): void {
    const v = this.selectedVariant();
    if (v && !this.quantityError()) {
      this.variantSelected.emit({ variant: v, quantity: this.quantity() });
    }
  }
}
