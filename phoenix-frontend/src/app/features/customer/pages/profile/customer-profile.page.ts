import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  effect,
  inject,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { ButtonModule }    from 'primeng/button';
import { DividerModule }   from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule }   from 'primeng/message';
import { SelectModule }    from 'primeng/select';
import { ToastModule }     from 'primeng/toast';
import { MessageService }  from 'primeng/api';

import { AuthService }      from '../../../../core/auth/auth.service';
import { ApiError }         from '../../../../shared/models/api-error.model';
import { CustomerService }  from '../../services/customer.service';
import { CustomerProfileDto } from '../../models/customer-address.model';
import { DatePipe } from '@angular/common';

/**
 * Page d'édition du profil client Phoenix.
 *
 * - Affiche l'email en lecture seule (non modifiable)
 * - Formulaire : prénom, nom, raison sociale, segment
 * - Update optimiste via CustomerService
 * - Synchro automatique du formulaire via effect() sur customerService.profile()
 */
@Component({
  selector: 'app-customer-profile-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    DatePipe,
    // PrimeNG
    InputTextModule,
    SelectModule,
    ButtonModule,
    ToastModule,
    MessageModule,
    DividerModule,
  ],
  template: `
    <p-toast />

    <div class="profile-page">

      <h2 class="profile-page__title">Mon profil</h2>

      @if (customerService.isLoading()) {
        <div class="profile-page__loading">Chargement du profil...</div>
      }

      @if (customerService.profile()) {

        <!-- Email (non modifiable) -->
        <div class="profile-info">
          <span class="profile-info__label">Email</span>
          <span class="profile-info__value">
            {{ customerService.profile()!.email }}
          </span>
          <span class="profile-info__note">
            L'adresse email ne peut pas être modifiée.
          </span>
        </div>

        <p-divider />

        <!-- Informations du compte -->
        <div class="profile-info">
          <span class="profile-info__label">Membre depuis</span>
          <span class="profile-info__value">
            {{ customerService.profile()!.createdAtUtc | date:'dd/MM/yyyy' }}
          </span>
        </div>

        <p-divider />

        <!-- Formulaire d'édition -->
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="profile-form">

          <div class="form-row">
            <div class="form-field">
              <label>Prénom *</label>
              <input
                pInputText
                formControlName="firstName"
                autocomplete="given-name"
                class="w-full"
                [class.ng-invalid]="isInvalid('firstName')" />
              @if (isInvalid('firstName')) {
                <small class="form-error">Prénom obligatoire.</small>
              }
            </div>
            <div class="form-field">
              <label>Nom *</label>
              <input
                pInputText
                formControlName="lastName"
                autocomplete="family-name"
                class="w-full"
                [class.ng-invalid]="isInvalid('lastName')" />
              @if (isInvalid('lastName')) {
                <small class="form-error">Nom obligatoire.</small>
              }
            </div>
          </div>

          <div class="form-field">
            <label>Raison sociale</label>
            <input
              pInputText
              formControlName="companyName"
              placeholder="Optionnel"
              autocomplete="organization"
              class="w-full" />
          </div>

          <div class="form-field">
            <label>Secteur d'activité *</label>
            <p-select
              formControlName="segment"
              [options]="segmentOptions"
              optionLabel="label"
              optionValue="value"
              placeholder="Choisissez votre activité"
              styleClass="w-full" />
          </div>

          <div class="profile-form__actions">
            <p-button
              type="submit"
              label="Enregistrer les modifications"
              [loading]="customerService.isSaving()"
              [disabled]="form.invalid && form.touched" />
          </div>

        </form>

      }

    </div>
  `,
  styleUrl: './customer-profile.page.scss'
})
export class CustomerProfilePage implements OnInit {
  protected readonly customerService = inject(CustomerService);
  protected readonly authService     = inject(AuthService);
  private   readonly messageService  = inject(MessageService);
  private   readonly fb              = inject(FormBuilder);

  protected readonly segmentOptions = [
    { value: 'FastFood',               label: '🍔 Fast Food & Burger' },
    { value: 'BakeryPastry',           label: '🥐 Boulangerie & Pâtisserie' },
    { value: 'JapaneseAsian',          label: '🍣 Japonais & Asiatique' },
    { value: 'BubbleTea',              label: '🧋 Bubble Tea' },
    { value: 'RetailCommerce',         label: '🛍️ Commerce & Retail' },
    { value: 'FoodTruck',              label: '🚚 Food Truck' },
    { value: 'Catering',               label: '🎉 Traiteur & Événementiel' },
    { value: 'ChocolateConfectionery', label: '🍫 Chocolaterie' },
    { value: 'PizzaShop',              label: '🍕 Pizzéria' },
    { value: 'Other',                  label: '📦 Autre activité' }
  ];

  protected readonly form = this.fb.group({
    firstName:   ['', [Validators.required, Validators.maxLength(100)]],
    lastName:    ['', [Validators.required, Validators.maxLength(100)]],
    companyName: ['', [Validators.maxLength(200)]],
    segment:     [null as string | null, [Validators.required]]
  });

  constructor() {
    // Réagit aux changements du signal profile() pour patcher le formulaire
    effect(() => {
      const profile = this.customerService.profile();
      if (profile) {
        this.patchForm(profile);
      }
    });
  }

  ngOnInit(): void {
    this.customerService.loadProfile();
  }

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { firstName, lastName, companyName, segment } = this.form.value;

    this.customerService.updateProfile({
      firstName:   firstName!,
      lastName:    lastName!,
      companyName: companyName || undefined,
      segment:     segment!
    }).subscribe({
      next: () => this.messageService.add({
        severity: 'success',
        summary:  'Profil mis à jour avec succès'
      }),
      error: (err: ApiError) => this.messageService.add({
        severity: 'error',
        summary:  'Erreur',
        detail:   err.message
      })
    });
  }

  protected isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl?.invalid && (ctrl.touched || ctrl.dirty));
  }

  private patchForm(profile: CustomerProfileDto): void {
    this.form.patchValue({
      firstName:   profile.firstName,
      lastName:    profile.lastName,
      companyName: profile.companyName ?? '',
      segment:     profile.segment
    });
  }
}
