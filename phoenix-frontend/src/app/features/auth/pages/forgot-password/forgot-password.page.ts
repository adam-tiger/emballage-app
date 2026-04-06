import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { ButtonModule }    from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule }   from 'primeng/message';
import { ToastModule }     from 'primeng/toast';

import { AuthService } from '../../../../core/auth/auth.service';

/**
 * Page "Mot de passe oublié".
 *
 * Sécurité : affiche toujours l'état de succès, qu'un compte existe ou non,
 * afin de ne pas révéler si une adresse e-mail est associée à un compte.
 */
@Component({
  selector: 'app-forgot-password-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    // PrimeNG
    InputTextModule,
    ButtonModule,
    MessageModule,
    ToastModule,
  ],
  template: `
    <p-toast position="top-right" />

    @if (!submitted()) {

      <div class="forgot-page">

        <div class="forgot-page__header">
          <h1>Mot de passe oublié</h1>
          <p>
            Entrez votre email et nous vous enverrons
            un lien de réinitialisation.
          </p>
        </div>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="forgot-form">

          <div class="form-field">
            <label for="forgot-email">Adresse email</label>
            <input
              id="forgot-email"
              pInputText
              formControlName="email"
              type="email"
              placeholder="pedro@monrestaurant.fr"
              autocomplete="email"
              class="w-full"
              [class.ng-invalid]="isInvalid('email')" />
            @if (isInvalid('email')) {
              <small class="form-error">Email invalide.</small>
            }
          </div>

          <p-button
            type="submit"
            label="Envoyer le lien"
            styleClass="w-full mt-4"
            [loading]="isLoading()" />

          <div class="forgot-page__back">
            <a routerLink="/connexion" class="link">
              ← Retour à la connexion
            </a>
          </div>

        </form>
      </div>

    } @else {

      <!-- État succès — affiché dans tous les cas -->
      <div class="forgot-success">
        <div class="forgot-success__icon">📧</div>
        <h2>Email envoyé !</h2>
        <p>
          Si un compte existe pour cette adresse,
          vous recevrez un email dans quelques minutes.
          Vérifiez vos spams si besoin.
        </p>
        <p-button
          label="Retour à la connexion"
          severity="secondary"
          [outlined]="true"
          styleClass="mt-4"
          routerLink="/connexion" />
      </div>

    }
  `,
  styleUrl: './forgot-password.page.scss'
})
export class ForgotPasswordPage {
  protected readonly authService = inject(AuthService);
  private   readonly fb          = inject(FormBuilder);

  protected readonly submitted  = signal(false);
  protected readonly isLoading  = signal(false);

  protected readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);

    this.authService.forgotPassword(this.form.value.email!)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next:  () => this.submitted.set(true),
        error: () => this.submitted.set(true)
        // Toujours afficher l'état succès — sécurité anti-énumération
      });
  }

  protected isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl?.invalid && (ctrl.touched || ctrl.dirty));
  }
}
