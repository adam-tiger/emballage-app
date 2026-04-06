import { ChangeDetectionStrategy, Component, inject, input, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { ButtonModule }    from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

import { AddAddressRequest } from '../../models/customer-address.model';

/**
 * Composant dumb — formulaire d'ajout d'une adresse de livraison.
 *
 * Émet `addressSubmitted` avec les données validées,
 * ou `cancelled` si l'utilisateur annule.
 * Toute la logique d'appel API est dans le composant parent.
 */
@Component({
  selector: 'app-add-address-form',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    InputTextModule,
    ButtonModule,
  ],
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="address-form">

      <!-- Libellé -->
      <div class="form-field">
        <label for="addr-label">Libellé *</label>
        <input
          id="addr-label"
          pInputText
          formControlName="label"
          placeholder="Mon restaurant, Entrepôt principal..."
          autocomplete="off"
          class="w-full"
          [class.ng-invalid]="isInvalid('label')" />
        @if (isInvalid('label')) {
          <small class="form-error">Libellé obligatoire.</small>
        }
      </div>

      <!-- Adresse -->
      <div class="form-field">
        <label for="addr-street">Adresse *</label>
        <input
          id="addr-street"
          pInputText
          formControlName="street"
          placeholder="12 rue de la Paix"
          autocomplete="street-address"
          class="w-full"
          [class.ng-invalid]="isInvalid('street')" />
        @if (isInvalid('street')) {
          <small class="form-error">Adresse obligatoire.</small>
        }
      </div>

      <!-- Code postal + Ville -->
      <div class="form-row">
        <div class="form-field">
          <label for="addr-postal">Code postal *</label>
          <input
            id="addr-postal"
            pInputText
            formControlName="postalCode"
            placeholder="75001"
            autocomplete="postal-code"
            maxlength="5"
            [class.ng-invalid]="isInvalid('postalCode')" />
          @if (isInvalid('postalCode')) {
            <small class="form-error">5 chiffres requis.</small>
          }
        </div>
        <div class="form-field">
          <label for="addr-city">Ville *</label>
          <input
            id="addr-city"
            pInputText
            formControlName="city"
            placeholder="Paris"
            autocomplete="address-level2"
            class="w-full"
            [class.ng-invalid]="isInvalid('city')" />
          @if (isInvalid('city')) {
            <small class="form-error">Ville obligatoire.</small>
          }
        </div>
      </div>

      <!-- Actions -->
      <div class="form-actions">
        <p-button
          label="Annuler"
          severity="secondary"
          [outlined]="true"
          type="button"
          (onClick)="cancelled.emit()" />
        <p-button
          label="Ajouter l'adresse"
          type="submit"
          [loading]="isSaving()" />
      </div>

    </form>
  `,
  styleUrl: './add-address-form.component.scss'
})
export class AddAddressFormComponent {
  private readonly fb = inject(FormBuilder);

  /** Vrai pendant la sauvegarde — contrôle l'état loading du bouton. */
  readonly isSaving = input<boolean>(false);

  /** Émis avec les données de l'adresse une fois le formulaire validé. */
  readonly addressSubmitted = output<AddAddressRequest>();

  /** Émis quand l'utilisateur clique "Annuler". */
  readonly cancelled = output<void>();

  protected readonly form = this.fb.group({
    label:      ['', [Validators.required, Validators.maxLength(100)]],
    street:     ['', [Validators.required, Validators.maxLength(200)]],
    city:       ['', [Validators.required, Validators.maxLength(100)]],
    postalCode: ['', [Validators.required, Validators.pattern('^\\d{5}$')]],
    country:    ['FR']
  });

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.addressSubmitted.emit(this.form.value as AddAddressRequest);
    this.form.reset({ country: 'FR' });
  }

  protected isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl?.invalid && (ctrl.touched || ctrl.dirty));
  }
}
