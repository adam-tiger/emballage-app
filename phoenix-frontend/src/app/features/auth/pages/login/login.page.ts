import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
  signal,
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { ButtonModule }   from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DividerModule }  from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule }  from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { ToastModule }    from 'primeng/toast';
import { MessageService } from 'primeng/api';

import { AuthService }  from '../../../../core/auth/auth.service';
import { ApiError }     from '../../../../shared/models/api-error.model';

/**
 * Page de connexion Phoenix.
 *
 * - Formulaire réactif email + password + rememberMe
 * - Erreurs affichées inline via p-message (signal authService.error())
 * - Succès affiché via Toast puis redirection selon rôle
 * - ReturnUrl préservé en queryParam pour reprendre la navigation
 */
@Component({
  selector: 'app-login-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    // PrimeNG
    InputTextModule,
    PasswordModule,
    CheckboxModule,
    ButtonModule,
    MessageModule,
    ToastModule,
    DividerModule,
  ],
  template: `
    <p-toast position="top-right" />

    <div class="login-page">

      <div class="login-page__header">
        <h1 class="login-page__title">Connexion</h1>
        <p class="login-page__sub">
          Pas encore de compte ?
          <a routerLink="/inscription" class="link">S'inscrire gratuitement</a>
        </p>
      </div>

      <!-- Erreur globale inline -->
      @if (authService.hasError()) {
        <p-message
          severity="error"
          [text]="authService.error()!.message"
          styleClass="w-full mb-4" />
      }

      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="login-form">

        <!-- Email -->
        <div class="form-field">
          <label for="email">Adresse email</label>
          <input
            id="email"
            type="email"
            pInputText
            formControlName="email"
            placeholder="pedro@monrestaurant.fr"
            autocomplete="email"
            class="w-full"
            [class.ng-invalid]="isInvalid('email')" />
          @if (isInvalid('email')) {
            <small class="form-error">{{ getFieldError('email') }}</small>
          }
        </div>

        <!-- Mot de passe -->
        <div class="form-field">
          <div class="form-field__label-row">
            <label for="password">Mot de passe</label>
            <a routerLink="/mot-de-passe-oublie" class="link link--small">
              Mot de passe oublié ?
            </a>
          </div>
          <p-password
            inputId="password"
            formControlName="password"
            placeholder="••••••••"
            [feedback]="false"
            [toggleMask]="true"
            styleClass="w-full"
            autocomplete="current-password" />
          @if (isInvalid('password')) {
            <small class="form-error">{{ getFieldError('password') }}</small>
          }
        </div>

        <!-- Rester connecté -->
        <div class="form-field form-field--inline">
          <p-checkbox
            inputId="rememberMe"
            formControlName="rememberMe"
            [binary]="true" />
          <label for="rememberMe">Rester connecté</label>
        </div>

        <p-button
          type="submit"
          label="Se connecter"
          styleClass="w-full"
          [loading]="authService.isLoading()"
          [disabled]="form.invalid && form.touched" />

      </form>

      <p-divider align="center">
        <span class="divider-text">ou</span>
      </p-divider>

      <div class="login-page__register">
        <p>Nouveau client ?</p>
        <p-button
          label="Créer un compte"
          severity="secondary"
          [outlined]="true"
          styleClass="w-full"
          routerLink="/inscription" />
      </div>

    </div>
  `,
  styleUrl: './login.page.scss'
})
export class LoginPage implements OnInit {
  protected readonly authService   = inject(AuthService);
  private   readonly router        = inject(Router);
  private   readonly route         = inject(ActivatedRoute);
  private   readonly messageService = inject(MessageService);
  private   readonly fb            = inject(FormBuilder);

  protected readonly returnUrl = signal<string>('/');

  protected readonly form = this.fb.group({
    email:      ['', [Validators.required, Validators.email]],
    password:   ['', [Validators.required, Validators.minLength(8)]],
    rememberMe: [false]
  });

  ngOnInit(): void {
    const url = this.route.snapshot.queryParams['returnUrl'] as string | undefined;
    this.returnUrl.set(url ?? '/');

    // Rediriger si déjà authentifié
    if (this.authService.isAuthenticated()) {
      this.redirectByRole();
    }

    // Effacer les erreurs précédentes
    this.authService.clearError();
  }

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password, rememberMe } = this.form.value;

    this.authService.login({
      email:      email!,
      password:   password!,
      rememberMe: rememberMe ?? false
    }).subscribe({
      next: response => {
        this.messageService.add({
          severity: 'success',
          summary:  `Bienvenue, ${response.user.firstName} !`,
          life:     2000
        });
        setTimeout(() => this.redirectByRole(), 500);
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
    if (ctrl.errors['minlength'])  return 'Minimum 8 caractères.';
    return '';
  }

  private redirectByRole(): void {
    if (this.authService.isAdmin() || this.authService.isEmployee()) {
      this.router.navigate(['/admin']);
    } else {
      this.router.navigate([this.returnUrl()]);
    }
  }
}
