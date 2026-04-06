import {
  APP_INITIALIZER,
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { providePrimeNG } from 'primeng/config';
import { ConfirmationService, MessageService } from 'primeng/api';
import Lara from '@primeng/themes/lara';
import { definePreset } from '@primeng/themes';

import { routes } from './app.routes';
import { jwtInterceptor } from './core/http/jwt.interceptor';
import { loadingInterceptor } from './core/http/loading.interceptor';
import { AuthService } from './core/auth/auth.service';

const PhoenixTheme = definePreset(Lara, {
  semantic: {
    primary: {
      50:  '#fef3ee',
      100: '#fde3d6',
      200: '#fbc6ac',
      300: '#f8a078',
      400: '#f47243',
      500: '#E8552A',
      600: '#d44220',
      700: '#b0311a',
      800: '#8e2719',
      900: '#732318',
      950: '#3e0f0b'
    }
  }
});

/**
 * Factory APP_INITIALIZER — restaure la session utilisateur depuis sessionStorage
 * au démarrage de l'application (SSR-compatible).
 */
function initAuth(authService: AuthService): () => void {
  return () => authService.initFromStorage();
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(
      routes,
      withInMemoryScrolling({ scrollPositionRestoration: 'top' })
    ),
    provideClientHydration(withEventReplay()),
    provideHttpClient(
      withFetch(),
      // jwtInterceptor AVANT loadingInterceptor :
      // gère l'injection du Bearer token ET le parsing des erreurs 401
      withInterceptors([jwtInterceptor, loadingInterceptor])
    ),
    MessageService,
    ConfirmationService,
    providePrimeNG({
      theme: {
        preset: PhoenixTheme,
        options: {
          darkModeSelector: '.dark',
          cssLayer: false
        }
      }
    }),
    // ── Auth initializer ───────────────────────────────────────────────────
    {
      provide: APP_INITIALIZER,
      useFactory: initAuth,
      deps: [AuthService],
      multi: true
    }
  ]
};
