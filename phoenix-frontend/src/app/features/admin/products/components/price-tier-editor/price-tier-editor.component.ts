import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { AddPriceTierRequest } from '../../models/add-price-tier.request';

@Component({
  selector: 'app-price-tier-editor',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, InputNumberModule, ButtonModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="tier-form">
      <div class="tier-form__row">
        <p-inputNumber
          formControlName="minQuantity"
          placeholder="Qté min"
          [min]="1"
          styleClass="tier-input" />
        <p-inputNumber
          formControlName="maxQuantity"
          placeholder="Qté max (vide = illimité)"
          styleClass="tier-input" />
        <p-inputNumber
          formControlName="unitPriceHT"
          placeholder="Prix HT/pcs (€)"
          [minFractionDigits]="4"
          [maxFractionDigits]="4"
          mode="currency"
          currency="EUR"
          locale="fr-FR"
          styleClass="tier-input" />
        <p-button
          label="Ajouter"
          type="submit"
          [loading]="isSaving()"
          [disabled]="form.invalid"
          size="small" />
      </div>
    </form>
  `,
  styleUrl: './price-tier-editor.component.scss'
})
export class PriceTierEditorComponent {
  variantId = input.required<string>();
  isSaving  = input<boolean>(false);

  tierSubmitted = output<AddPriceTierRequest>();

  readonly form = new FormGroup({
    minQuantity: new FormControl<number>(1, [
      Validators.required,
      Validators.min(1)
    ]),
    maxQuantity: new FormControl<number | null>(null),
    unitPriceHT: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(0.001)
    ])
  });

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.getRawValue();
    this.tierSubmitted.emit({
      minQuantity: raw.minQuantity!,
      maxQuantity: raw.maxQuantity ?? undefined,
      unitPriceHT: raw.unitPriceHT!
    });
    this.form.reset({ minQuantity: 1, maxQuantity: null, unitPriceHT: null });
  }
}
