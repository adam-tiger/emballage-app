import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';

import { ButtonModule }    from 'primeng/button';
import { CheckboxModule }  from 'primeng/checkbox';
import { DividerModule }   from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule }   from 'primeng/message';
import { PasswordModule }  from 'primeng/password';
import { SelectModule }    from 'primeng/select';
import { ToastModule }     from 'primeng/toast';
import { MessageService }  from 'primeng/api';

import { AuthService } from '../../../../core/auth/auth.service';
import { ApiError }    from '../../../../shared/models/api-error.model';

/** Validator de groupe : vérifie que password === confirmPassword. */
const passwordMatchValidator: ValidatorFn = (
  group: AbstractControl
): ValidationErrors | null => {
  const password        = group.get('password')?.value as string | null;
  const confirmPassword = group.get('confirmPassword')?.value as string | null;
  return password && confirmPassword && password !== confirmPassword
    ? { passwordMismatch: true }
    : null;
};

/**
 * Page d'inscription Phoenix — formulaire complet B2B.
 *
 * - Champs : prénom, nom, email, raison sociale, segment, mot de passe, CGU
 * - Validator cross-field passwordMatch
 * - Segmentation par activité (10 options avec emoji)
 * - Redirection vers espace-client après création
 */
@Component({
  selector: 'app-register-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    // PrimeNG
    InputTextModule,
    PasswordModule,
    SelectModule,
    ButtonModule,
    MessageModule,
    ToastModule,
    CheckboxModule,
    DividerModule,
  ],
  template: `
    <p-toast position="top-right" />

    <div class="register-page">

      <div class="register-page__header">
        <h1>Créer un compte</h1>
        <p>
          Déjà client ?
          <a routerLink="/connexion" class="link">Se connecter</a>
        </p>
      </div>

      <!-- Erreur globale inline -->
      @if (authService.hasError()) {
        <p-message
          severity="error"
          [text]="authService.error()!.message"
          styleClass="w-full" />
      }

      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="register-form">

        <!-- Prénom / Nom -->
        <div class="form-row">
          <div class="form-field">
            <label>Prénom *</label>
            <input
              pInputText
              formControlName="firstName"
              placeholder="Pedro"
              autocomplete="given-name"
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
              placeholder="Martinez"
              autocomplete="family-name"
              [class.ng-invalid]="isInvalid('lastName')" />
            @if (isInvalid('lastName')) {
              <small class="form-error">Nom obligatoire.</small>
            }
          </div>
        </div>

        <!-- Email -->
        <div class="form-field">
          <label>Email professionnel *</label>
          <input
            pInputText
            formControlName="email"
            type="email"
            placeholder="pedro@monrestaurant.fr"
            autocomplete="email"
            class="w-full"
            [class.ng-invalid]="isInvalid('email')" />
          @if (isInvalid('email')) {
            <small class="form-error">{{ getFieldError('email') }}</small>
          }
        </div>

        <!-- Raison sociale -->
        <div class="form-field">
          <label>Raison sociale (optionnel)</label>
          <input
            pInputText
            formControlName="companyName"
            placeholder="Burger du Coin SARL"
            autocomplete="organization"
            class="w-full" />
        </div>

        <!-- Secteur d'activité -->
        <div class="form-field">
          <label>Secteur d'activité *</label>
          <p-select
            formControlName="segment"
            [options]="segmentOptions"
            optionLabel="label"
            optionValue="value"
            placeholder="Choisissez votre activité"
            styleClass="w-full"
            [class.ng-invalid]="isInvalid('segment')" />
          @if (isInvalid('segment')) {
            <small class="form-error">Veuillez sélectionner votre activité.</small>
          }
        </div>

        <p-divider />

        <!-- Mot de passe -->
        <div class="form-field">
          <label>Mot de passe *</label>
          <p-password
            formControlName="password"
            placeholder="Minimum 8 caractères"
            [toggleMask]="true"
            [feedback]="true"
            styleClass="w-full"
            promptLabel="Entrez un mot de passe"
            weakLabel="Faible"
            mediumLabel="Moyen"
            strongLabel="Fort"
            autocomplete="new-password" />
          @if (isInvalid('password')) {
            <small class="form-error">{{ getFieldError('password') }}</small>
          }
        </div>

        <!-- Confirmation mot de passe -->
        <div class="form-field">
          <label>Confirmer le mot de passe *</label>
          <p-password
            formControlName="confirmPassword"
            placeholder="Répétez votre mot de passe"
            [toggleMask]="true"
            [feedback]="false"
            styleClass="w-full"
            autocomplete="new-password" />
          @if (isInvalid('confirmPassword') || form.errors?.['passwordMismatch']) {
            <small class="form-error">
              Les mots de passe ne correspondent pas.
            </small>
          }
        </div>

        <!-- CGU -->
        <div class="form-field form-field--inline">
          <p-checkbox
            formControlName="acceptTerms"
            [binary]="true"
            inputId="terms" />
          <label for="terms" class="terms-label">
            J'accepte les
            <a href="/cgu" target="_blank" class="link">
              conditions générales d'utilisation
            </a>
          </label>
        </div>
        @if (isInvalid('acceptTerms')) {
          <small class="form-error">
            Vous devez accepter les CGU pour continuer.
          </small>
        }

        <p-button
          type="submit"
          label="Créer mon compte"
          styleClass="w-full"
          [loading]="authService.isLoading()"
          [disabled]="form.invalid && form.touched" />

      </form>
    </div>
  `,
  styleUrl: './register.page.scss'
})
export class RegisterPage implements OnInit {
  protected readonly authService    = inject(AuthService);
  private   readonly router         = inject(Router);
  private   readonly messageService = inject(MessageService);
  private   readonly fb             = inject(FormBuilder);

  /** Options de segmentation B2B avec emoji et libellés français. */
  protected readonly segmentOptions = [
    { value: 'FastFood',             label: '🍔 Fast Food & Burger' },
    { value: 'BakeryPastry',         label: '🥐 Boulangerie & Pâtisserie' },
    { value: 'JapaneseAsian',        label: '🍣 Japonais & Asiatique' },
    { value: 'BubbleTea',            label: '🧋 Bubble Tea' },
    { value: 'RetailCommerce',       label: '🛍️ Commerce & Retail' },
    { value: 'FoodTruck',            label: '🚚 Food Truck' },
    { value: 'Catering',             label: '🎉 Traiteur & Événementiel' },
    { value: 'ChocolateConfectionery', label: '🍫 Chocolaterie' },
    { value: 'PizzaShop',            label: '🍕 Pizzéria' },
    { value: 'Other',                label: '📦 Autre activité' }
  ] as const;

  protected readonly form = this.fb.group(
    {
      email:           ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
      password:        ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(100),
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$')
      ]],
      confirmPassword: ['', [Validators.required]],
      firstName:       ['', [Validators.required, Validators.maxLength(100)]],
      lastName:        ['', [Validators.required, Validators.maxLength(100)]],
      companyName:     ['', [Validators.maxLength(200)]],
      segment:         [null as string | null, [Validators.required]],
      acceptTerms:     [false, [Validators.requiredTrue]]
    },
    { validators: passwordMatchValidator }
  );

  ngOnInit(): void {
    this.authService.clearError();
  }

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password, confirmPassword,
            firstName, lastName, companyName, segment } = this.form.value;

    this.authService.register({
      email:           email!,
      password:        password!,
      confirmPassword: confirmPassword!,
      firstName:       firstName!,
      lastName:        lastName!,
      companyName:     companyName || undefined,
      segment:         segment!
    }).subscribe({
      next: response => {
        this.messageService.add({
          severity: 'success',
          summary:  `Compte créé ! Bienvenue, ${response.user.firstName} !`
        });
        setTimeout(() => this.router.navigate(['/espace-client']), 800);
      },
      error: (_err: ApiError) => {
        // Erreur déjà dans authService.error() → affichée par p-message inline
      }
    });
  }

  protected isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl?.invalid && (ctrl.touched || ctrl.dirty));
  }

  protected getFieldError(field: string): string {
    const ctrl = this.form.get(field);
    if (!ctrl?.errors) return '';
    if (ctrl.errors['required'])   return 'Ce champ est obligatoire.';
    if (ctrl.errors['email'])      return 'Adresse email invalide.';
    if (ctrl.errors['minlength'])  return `Minimum ${ctrl.errors['minlength'].requiredLength} caractères.`;
    if (ctrl.errors['maxlength'])  return `Maximum ${ctrl.errors['maxlength'].requiredLength} caractères.`;
    if (ctrl.errors['pattern'])    return 'Le mot de passe doit contenir une majuscule, une minuscule et un chiffre.';
    return '';
  }
}
