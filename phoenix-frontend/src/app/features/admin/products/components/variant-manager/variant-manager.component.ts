import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { NgIf } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { ButtonModule } from 'primeng/button';
import { PanelModule } from 'primeng/panel';
import { DividerModule } from 'primeng/divider';
import { PriceTierEditorComponent } from '../price-tier-editor/price-tier-editor.component';
import { ProductVariant, PRINT_SIDE_LABELS, COLOR_COUNT_LABELS } from '../../../../catalog/models/product-variant.model';
import { formatEur } from '../../../../catalog/models/price-tier.model';
import { AddVariantRequest } from '../../models/add-variant.request';
import { AddPriceTierRequest } from '../../models/add-price-tier.request';

@Component({
  selector: 'app-variant-manager',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    NgIf,
    InputTextModule,
    InputNumberModule,
    SelectModule,
    ButtonModule,
    PanelModule,
    DividerModule,
    PriceTierEditorComponent
  ],
  template: `
    <!-- Variantes existantes -->
    @for (variant of variants(); track variant.id) {
      <p-panel
        [header]="variant.nameFr + ' — ' + variant.sku"
        [toggleable]="true"
        [collapsed]="true"
        styleClass="mb-3">

        <div class="variant-info">
          <span class="variant-info__tag">
            MOQ : {{ variant.minimumOrderQuantity }} pcs
          </span>
          <span class="variant-info__tag">
            {{ PRINT_SIDE_LABELS[variant.printSide] }}
          </span>
          <span class="variant-info__tag">
            {{ COLOR_COUNT_LABELS[variant.colorCount] }}
          </span>
        </div>

        <!-- Paliers existants -->
        <div class="tier-list">
          <div class="tier-list__title">Paliers tarifaires</div>
          @for (tier of variant.priceTiers; track tier.id) {
            <div class="tier-row">
              <span class="tier-row__range">
                {{ tier.minQuantity }}
                @if (tier.maxQuantity) {
                  – {{ tier.maxQuantity }}
                } @else {
                  et +
                }
                pcs
              </span>
              <span class="tier-row__price">
                {{ formatEur(tier.unitPriceHT) }} / pcs
              </span>
            </div>
          }
          @if (variant.priceTiers.length === 0) {
            <p class="tier-list__empty">Aucun palier — ajoutez-en un ci-dessous.</p>
          }
        </div>

        <!-- Ajout palier -->
        <app-price-tier-editor
          [variantId]="variant.id"
          (tierSubmitted)="onPriceTierSubmit(variant.id, $event)" />

      </p-panel>
    }

    <!-- Formulaire nouvelle variante -->
    <p-panel header="Ajouter une variante" styleClass="mt-3">
      <form [formGroup]="variantForm" (ngSubmit)="onVariantSubmit()"
            class="variant-form">

        <div class="variant-form__grid">
          <div class="admin-form__field">
            <label>SKU variante *</label>
            <input pInputText formControlName="sku"
                   placeholder="ex: SAC-BRUN-22x10x28-70G" />
          </div>
          <div class="admin-form__field">
            <label>Nom (FR) *</label>
            <input pInputText formControlName="nameFr"
                   placeholder="ex: Brun 70G" />
          </div>
          <div class="admin-form__field">
            <label>MOQ (quantité minimum) *</label>
            <p-inputNumber
              formControlName="minimumOrderQuantity"
              [min]="1"
              placeholder="ex: 250" />
          </div>
          <div class="admin-form__field">
            <label>Impression</label>
            <p-select
              formControlName="printSide"
              [options]="printSideOptions"
              optionLabel="label"
              optionValue="value" />
          </div>
          <div class="admin-form__field">
            <label>Couleurs</label>
            <p-select
              formControlName="colorCount"
              [options]="colorCountOptions"
              optionLabel="label"
              optionValue="value" />
          </div>
        </div>

        <p-button
          label="Créer la variante"
          type="submit"
          [disabled]="variantForm.invalid" />

      </form>
    </p-panel>
  `,
  styleUrl: './variant-manager.component.scss'
})
export class VariantManagerComponent {
  productId = input.required<string>();
  variants  = input.required<ProductVariant[]>();

  variantAdded   = output<AddVariantRequest>();
  priceTierAdded = output<{ variantId: string; tier: AddPriceTierRequest }>();

  readonly PRINT_SIDE_LABELS  = PRINT_SIDE_LABELS;
  readonly COLOR_COUNT_LABELS = COLOR_COUNT_LABELS;
  readonly formatEur          = formatEur;

  readonly printSideOptions = [
    { label: 'Recto uniquement',   value: 'SingleSide' },
    { label: 'Recto-verso ✦',      value: 'DoubleSide' }
  ];

  readonly colorCountOptions = [
    { label: '1 couleur',      value: 'One' },
    { label: '2 couleurs',     value: 'Two' },
    { label: '3 couleurs',     value: 'Three' },
    { label: '4 couleurs CMJN', value: 'FourCMYK' }
  ];

  readonly variantForm = new FormGroup({
    sku:                    new FormControl('', [Validators.required, Validators.maxLength(50)]),
    nameFr:                 new FormControl('', [Validators.required, Validators.maxLength(200)]),
    minimumOrderQuantity:   new FormControl<number | null>(null, [Validators.required, Validators.min(1)]),
    printSide:              new FormControl('SingleSide', Validators.required),
    colorCount:             new FormControl('One', Validators.required)
  });

  onVariantSubmit(): void {
    if (this.variantForm.invalid) {
      this.variantForm.markAllAsTouched();
      return;
    }
    this.variantAdded.emit(this.variantForm.getRawValue() as AddVariantRequest);
    this.variantForm.reset({
      sku: '', nameFr: '', minimumOrderQuantity: null,
      printSide: 'SingleSide', colorCount: 'One'
    });
  }

  onPriceTierSubmit(variantId: string, tier: AddPriceTierRequest): void {
    this.priceTierAdded.emit({ variantId, tier });
  }
}
